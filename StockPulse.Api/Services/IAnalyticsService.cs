using StockPulse.Api.DTOs;

namespace StockPulse.Api.Services;

public interface IAnalyticsService
{
    Task<List<TopMoverDto>> GetTopMoversAsync(int limit);
}
