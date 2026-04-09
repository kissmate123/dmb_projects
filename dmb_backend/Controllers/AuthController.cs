using dmb_backend.DTOs;
using dmb_backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace dmb_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var email = (dto.Email ?? "").Trim();
        var userName = (dto.UserName ?? "").Trim();
        var password = dto.Password ?? "";

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { message = "Email megadása kötelező." });

        if (string.IsNullOrWhiteSpace(userName))
            return BadRequest(new { message = "Felhasználónév megadása kötelező." });

        if (string.IsNullOrWhiteSpace(password))
            return BadRequest(new { message = "Jelszó megadása kötelező." });

        var existingByEmail = await _userManager.FindByEmailAsync(email);
        if (existingByEmail != null)
            return BadRequest(new { message = "Ez az email már létezik." });

        var existingByUserName = await _userManager.FindByNameAsync(userName);
        if (existingByUserName != null)
            return BadRequest(new { message = "Ez a felhasználónév már létezik." });

        var user = new ApplicationUser
        {
            Email = email,
            UserName = userName
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Sikertelen regisztráció.",
                errors = result.Errors.Select(e => e.Description)
            });
        }

        return Ok(new { message = "Sikeres regisztráció." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var identifier = (dto.Identifier ?? "").Trim();
        var password = dto.Password ?? "";

        if (string.IsNullOrWhiteSpace(identifier))
            return BadRequest(new { message = "Email vagy felhasználónév megadása kötelező." });

        if (string.IsNullOrWhiteSpace(password))
            return BadRequest(new { message = "Jelszó megadása kötelező." });

        var user = await _userManager.FindByEmailAsync(identifier);
        if (user == null)
            user = await _userManager.FindByNameAsync(identifier);

        if (user == null)
            return Unauthorized(new { message = "Hibás email/felhasználónév vagy jelszó." });

        var ok = await _userManager.CheckPasswordAsync(user, password);
        if (!ok)
            return Unauthorized(new { message = "Hibás email/felhasználónév vagy jelszó." });

        return Ok(CreateJwtToken(user));
    }

    private AuthResponseDto CreateJwtToken(ApplicationUser user)
    {
        var key = _config["Jwt:Key"]!;
        var issuer = _config["Jwt:Issuer"]!;
        var audience = _config["Jwt:Audience"]!;
        var expireMinutes = int.Parse(_config["Jwt:ExpireMinutes"]!);

        var isAdmin =
            string.Equals(user.Email, "admin44@gmail.com", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase);

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName ?? ""),
        new Claim("is_admin", isAdmin ? "true" : "false")
    };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(expireMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new AuthResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            expires
        );
    }

}
