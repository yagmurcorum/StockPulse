using Microsoft.AspNetCore.Mvc;
using StockPulse.Api.Services;

namespace StockPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StocksController : ControllerBase
{
    private readonly IStockService _stockService;

    public StocksController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stocks = await _stockService.GetAllAsync();

        return Ok(stocks);
    }

    [HttpGet("{symbol}")]
    public async Task<IActionResult> GetBySymbol(string symbol)
    {
        var stock = await _stockService.GetBySymbolAsync(symbol);

        if (stock is null)
        {
            return NotFound(new { message = $"Stock '{symbol}' was not found." });
        }

        return Ok(stock);
    }

    [HttpPost("track/{symbol}")]
    public async Task<IActionResult> TrackStock(string symbol)
    {
        try
        {
            var stock = await _stockService.TrackStockAsync(symbol);

            return Ok(stock);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{symbol}/refresh")]
    public async Task<IActionResult> RefreshStock(string symbol)
    {
        try
        {
            var stock = await _stockService.RefreshStockAsync(symbol);

            return Ok(stock);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{symbol}")]
    public async Task<IActionResult> DeleteStock(string symbol)
    {
        var deleted = await _stockService.DeleteStockAsync(symbol);

        if (!deleted)
        {
            return NotFound(new { message = $"Stock '{symbol}' was not found." });
        }

        return NoContent();
    }
}
