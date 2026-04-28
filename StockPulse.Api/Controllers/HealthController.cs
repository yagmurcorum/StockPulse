using Microsoft.AspNetCore.Mvc;

namespace StockPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "Healthy",
            application = "StockPulse.Api",
            timestampUtc = DateTime.UtcNow
        });
    }
}
