using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace dmb_backend.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.FindFirstValue(ClaimTypes.Name);

        return Ok(new
        {
            message = "Be vagy jelentkezve.",
            userId,
            userName
        });
    }
}
