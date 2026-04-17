using dmb_backend.Controllers;
using dmb_backend.DTOs;
using dmb_backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace dmb_backend.Tests;

public class AuthControllerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c["Jwt:Key"]).Returns("this-is-a-super-secret-key-that-is-very-long-for-testing-purposes");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
        _configMock.Setup(c => c["Jwt:ExpireMinutes"]).Returns("60");

        _controller = new AuthController(_userManagerMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task Register_ValidInput_ReturnsOkResult()
    {
        // Arrange
        var registerDto = new RegisterDto("test@example.com", "testuser", "Test@123!");
        _userManagerMock.Setup(um => um.FindByEmailAsync("test@example.com"))
            .ReturnsAsync((ApplicationUser)null);
        _userManagerMock.Setup(um => um.FindByNameAsync("testuser"))
            .ReturnsAsync((ApplicationUser)null);
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Register_EmptyEmail_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto("", "testuser", "Test@123!");

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badResult.Value);
    }

    [Fact]
    public async Task Register_EmptyPassword_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto("test@example.com", "testuser", "");

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badResult.Value);
    }

    [Fact]
    public async Task Register_ExistingEmail_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto("test@example.com", "testuser", "Test@123!");
        var existingUser = new ApplicationUser { Email = "test@example.com" };
        _userManagerMock.Setup(um => um.FindByEmailAsync("test@example.com"))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badResult.Value);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsAuthResponseDto()
    {
        // Arrange
        var loginDto = new LoginDto("testuser", "Test@123!");
        var user = new ApplicationUser 
        { 
            Id = "user-123",
            UserName = "testuser", 
            Email = "test@example.com" 
        };

        _userManagerMock.Setup(um => um.FindByEmailAsync("testuser"))
            .ReturnsAsync((ApplicationUser)null);
        _userManagerMock.Setup(um => um.FindByNameAsync("testuser"))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, "Test@123!"))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var authResponse = okResult.Value as AuthResponseDto;
        Assert.NotNull(authResponse);
        Assert.False(string.IsNullOrEmpty(authResponse.Token));
    }

    [Fact]
    public async Task Login_EmptyIdentifier_ReturnsBadRequest()
    {
        // Arrange
        var loginDto = new LoginDto("", "Test@123!");

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var badResult = result.Result as BadRequestObjectResult;
        Assert.NotNull(badResult);
    }

    [Fact]
    public async Task Login_InvalidUser_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto("nonexistent", "Test@123!");
        _userManagerMock.Setup(um => um.FindByEmailAsync("nonexistent"))
            .ReturnsAsync((ApplicationUser)null);
        _userManagerMock.Setup(um => um.FindByNameAsync("nonexistent"))
            .ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        Assert.NotNull(unauthorizedResult);
    }

    [Fact]
    public async Task Login_IncorrectPassword_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto("testuser", "WrongPassword!");
        var user = new ApplicationUser 
        { 
            Id = "user-123",
            UserName = "testuser", 
            Email = "test@example.com" 
        };

        _userManagerMock.Setup(um => um.FindByNameAsync("testuser"))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, "WrongPassword!"))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        Assert.NotNull(unauthorizedResult);
    }

    [Fact]
    public async Task Login_TokenContainsCorrectClaims()
    {
        // Arrange
        var loginDto = new LoginDto("admin", "Admin@123!");
        var user = new ApplicationUser 
        { 
            Id = "admin-123",
            UserName = "admin", 
            Email = "admin44@gmail.com" 
        };

        _userManagerMock.Setup(um => um.FindByNameAsync("admin"))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, "Admin@123!"))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var authResponse = okResult.Value as AuthResponseDto;
        Assert.NotNull(authResponse);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadToken(authResponse.Token) as JwtSecurityToken;
        Assert.NotNull(token);
        
        var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");
        Assert.NotNull(emailClaim);
        Assert.Equal("admin44@gmail.com", emailClaim.Value);

        var isAdminClaim = token.Claims.FirstOrDefault(c => c.Type == "is_admin");
        Assert.NotNull(isAdminClaim);
        Assert.Equal("true", isAdminClaim.Value);
    }

    [Fact]
    public async Task Register_EmptyUsername_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto("test@example.com", "", "Test@123!");

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badResult.Value);
    }
}
