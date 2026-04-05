using CapstoneGenerator.API.Data;
using CapstoneGenerator.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CapstoneGenerator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly AnalyticsDbContext _context;

    public ReviewController(AnalyticsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reviews = await _context.Reviews
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        return Ok(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Review review)
    {
        if (string.IsNullOrWhiteSpace(review.Message))
            return BadRequest("Message is required.");

        if (review.Rating < 1 || review.Rating > 5)
            return BadRequest("Rating must be between 1 and 5.");

        review.Name = string.IsNullOrWhiteSpace(review.Name) ? "Anonymous" : review.Name.Trim();
        review.Message = review.Message.Trim();
        review.CreatedAt = DateTime.UtcNow;

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        return Ok(review);
    }
}