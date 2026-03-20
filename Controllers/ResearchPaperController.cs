using CapstoneGenerator.API.Models;
using CapstoneGenerator.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneGenerator.API.Controllers;

[ApiController]
[Route("api/capstone")]
public class ResearchPaperController : ControllerBase
{
    private readonly ResearchPaperService _researchPaperService;
    private readonly ILogger<ResearchPaperController> _logger;

    public ResearchPaperController(ResearchPaperService researchPaperService, ILogger<ResearchPaperController> logger)
    {
        _researchPaperService = researchPaperService;
        _logger = logger;
    }

    [HttpPost("research-paper")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerateResearchPaper([FromBody] CapstoneResponse capstone)
    {
        if (string.IsNullOrWhiteSpace(capstone.Title))
            return BadRequest("Capstone title is required.");

        try
        {
            var content = await _researchPaperService.GenerateAsync(capstone);
            return Ok(new { content });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Groq API request failed for research paper.");
            return StatusCode(502, new { error = "Failed to reach Groq API.", detail = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during research paper generation.");
            return StatusCode(500, new { error = "An unexpected error occurred.", detail = ex.Message });
        }
    }
}