using dmb_backend.DTOs;

namespace dmb_backend.Tests;

public class DTOsTests
{
    [Fact]
    public void RegisterDto_CanBeCreatedWithValidData()
    {
        // Arrange & Act
        var dto = new RegisterDto("test@example.com", "testuser", "Password123!");

        // Assert
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal("testuser", dto.UserName);
        Assert.Equal("Password123!", dto.Password);
    }

    [Fact]
    public void LoginDto_CanBeCreatedWithValidData()
    {
        // Arrange & Act
        var dto = new LoginDto("user@example.com", "Password123!");

        // Assert
        Assert.Equal("user@example.com", dto.Identifier);
        Assert.Equal("Password123!", dto.Password);
    }

    [Fact]
    public void NewsDto_CanBeCreatedWithValidData()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;

        // Act
        var dto = new NewsDto(1, "Test Title", "Test Content", createdAt);

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("Test Title", dto.Title);
        Assert.Equal("Test Content", dto.Text);
        Assert.Equal(createdAt, dto.CreatedAtUtc);
    }

    [Fact]
    public void NewsCreateDto_CanBeCreatedWithValidData()
    {
        // Arrange & Act
        var dto = new NewsCreateDto("New Title", "New Content");

        // Assert
        Assert.Equal("New Title", dto.Title);
        Assert.Equal("New Content", dto.Text);
    }

    [Fact]
    public void AuthResponseDto_ContainsTokenAndExpires()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
        var expires = DateTime.UtcNow.AddMinutes(60);

        // Act
        var dto = new AuthResponseDto(token, expires);

        // Assert
        Assert.Equal(token, dto.Token);
        Assert.Equal(expires, dto.Expires);
    }

    [Fact]
    public void DTOs_AreRecordTypes()
    {
        // Ensure DTOs work as record types with value equality
        var dto1 = new LoginDto("test@example.com", "password");
        var dto2 = new LoginDto("test@example.com", "password");

        // Records use structural equality
        Assert.Equal(dto1, dto2);
    }

    [Fact]
    public void RegisterDto_WithEmptyStrings()
    {
        // Arrange & Act
        var dto = new RegisterDto("", "", "");

        // Assert
        Assert.Empty(dto.Email ?? "");
        Assert.Empty(dto.UserName ?? "");
        Assert.Empty(dto.Password ?? "");
    }

    [Fact]
    public void NewsCreateDto_WithEmptyStrings()
    {
        // Arrange & Act
        var dto = new NewsCreateDto("", "");

        // Assert
        Assert.Empty(dto.Title);
        Assert.Empty(dto.Text);
    }

    [Fact]
    public void AuthResponseDto_TokenIsNotEmpty()
    {
        // Arrange
        var token = "test-token-12345";
        var expires = DateTime.UtcNow;

        // Act
        var dto = new AuthResponseDto(token, expires);

        // Assert
        Assert.NotEmpty(dto.Token);
        Assert.False(string.IsNullOrWhiteSpace(dto.Token));
    }
}
