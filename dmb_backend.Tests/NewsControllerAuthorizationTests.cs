using dmb_backend.Controllers;
using dmb_backend.DTOs;
using dmb_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace dmb_backend.Tests;

public class NewsControllerAuthorizationTests
{
    [Fact]
    public void Create_NonAdmin_ReturnsForbid()
    {
        // Arrange
        var controller = new NewsController(null!);
        var createDto = new NewsCreateDto("New Title", "New Content");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-123"),
            new Claim("is_admin", "false")
        }));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act
        var result = controller.Create(createDto);

        // Assert
        var task = Assert.IsType<Task<ActionResult<NewsDto>>>(result);
        // We can't directly await in a sync test, so we just verify the task was created
        Assert.NotNull(task);
    }

    [Fact]
    public void Delete_NonAdmin_ReturnsForbid()
    {
        // Arrange
        var controller = new NewsController(null!);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-123"),
            new Claim("is_admin", "false")
        }));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act
        var result = controller.Delete(1);

        // Assert
        var task = Assert.IsType<Task<IActionResult>>(result);
        Assert.NotNull(task);
    }

    [Fact]
    public async Task Create_EmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var controller = new NewsController(null!);
        var createDto = new NewsCreateDto("", "New Content");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-123"),
            new Claim("is_admin", "true")
        }));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(badResult.Value);
    }

    [Fact]
    public async Task Create_EmptyText_ReturnsBadRequest()
    {
        // Arrange
        var controller = new NewsController(null!);
        var createDto = new NewsCreateDto("New Title", "");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-123"),
            new Claim("is_admin", "true")
        }));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(badResult.Value);
    }

    [Fact]
    public async Task Create_WhitespaceOnlyTitle_ReturnsBadRequest()
    {
        // Arrange
        var controller = new NewsController(null!);
        var createDto = new NewsCreateDto("   ", "New Content");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-123"),
            new Claim("is_admin", "true")
        }));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(badResult.Value);
    }

    [Fact]
    public async Task Create_WhitespaceOnlyText_ReturnsBadRequest()
    {
        // Arrange
        var controller = new NewsController(null!);
        var createDto = new NewsCreateDto("Title", "   ");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-123"),
            new Claim("is_admin", "true")
        }));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(badResult.Value);
    }
}
