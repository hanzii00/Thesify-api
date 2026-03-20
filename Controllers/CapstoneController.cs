using CapstoneGenerator.API.Models;
using CapstoneGenerator.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneGenerator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CapstoneController : ControllerBase
{
    private readonly GroqService _groqService;
    private readonly ILogger<CapstoneController> _logger;

    public CapstoneController(GroqService groqService, ILogger<CapstoneController> logger)
    {
        _groqService = groqService;
        _logger = logger;
    }

    [HttpPost("generate")]
    [ProducesResponseType(typeof(CapstoneResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Generate([FromBody] CapstoneRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Course))
            return BadRequest("Course is required.");

        if (string.IsNullOrWhiteSpace(request.Difficulty))
            return BadRequest("Difficulty is required.");

        if (request.Interests == null || request.Interests.Count == 0)
            return BadRequest("At least one interest is required.");

        try
        {
            var result = await _groqService.GenerateAsync(request);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Groq API request failed.");
            return StatusCode(502, new { error = "Failed to reach Groq API.", detail = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during capstone generation.");
            return StatusCode(500, new { error = "An unexpected error occurred.", detail = ex.Message });
        }
    }
}