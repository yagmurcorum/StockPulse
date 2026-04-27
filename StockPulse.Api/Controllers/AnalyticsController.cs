using Microsoft.AspNetCore.Mvc;
using StockPulse.Api.Services;


namespace StockPulse.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("top-movers")]
    public async Task<IActionResult> GetTopMovers([FromQuery] int limit = 5)
    {
        var result = await _analyticsService.GetTopMoversAsync(limit);

        return Ok(result);
    }
}
