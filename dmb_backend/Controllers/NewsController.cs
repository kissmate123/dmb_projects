using System.Security.Claims;
using dmb_backend.Data;
using dmb_backend.DTOs;
using dmb_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dmb_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly AppDbContext _db;

    public NewsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NewsDto>>> GetAll()
    {
        var items = await _db.NewsItems
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new NewsDto(
                x.Id,
                x.Title,
                x.Text,
                x.CreatedAtUtc
            ))
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NewsDto>> GetById(int id)
    {
        var item = await _db.NewsItems
            .Where(x => x.Id == id)
            .Select(x => new NewsDto(
                x.Id,
                x.Title,
                x.Text,
                x.CreatedAtUtc
            ))
            .FirstOrDefaultAsync();

        if (item is null)
            return NotFound(new { message = "A hír nem található." });

        return Ok(item);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<NewsDto>> Create([FromBody] NewsCreateDto dto)
    {
        if (!IsAdmin())
            return Forbid();

        if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Text))
            return BadRequest(new { message = "A cím és a szöveg megadása kötelező." });

        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");

        var item = new NewsItem
        {
            Title = dto.Title.Trim(),
            Text = dto.Text.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        _db.NewsItems.Add(item);
        await _db.SaveChangesAsync();

        var result = new NewsDto(
            item.Id,
            item.Title,
            item.Text,
            item.CreatedAtUtc
        );

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, result);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdmin())
            return Forbid();

        var item = await _db.NewsItems.FindAsync(id);
        if (item is null)
            return NotFound(new { message = "A hír nem található." });

        _db.NewsItems.Remove(item);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private bool IsAdmin()
    {
        var claim =
            User.FindFirst("is_admin")?.Value ??
            User.FindFirst(ClaimTypes.Role)?.Value ??
            User.FindFirst("role")?.Value;

        if (string.IsNullOrWhiteSpace(claim))
            return false;

        return claim.Equals("true", StringComparison.OrdinalIgnoreCase) ||
               claim.Equals("admin", StringComparison.OrdinalIgnoreCase);
    }
}