using StockPulse.Api.DTOs;
using StockPulse.Api.Repositories;

namespace StockPulse.Api.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IStockRepository _stockRepository;

    public AnalyticsService(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<List<TopMoverDto>> GetTopMoversAsync(int limit)
    {
        if (limit <= 0)
        {
            limit = 5;
        }

        var stocks = await _stockRepository.GetAllAsync();

        return stocks
            .Select(stock =>
            {
                var latestSnapshot = stock.PriceSnapshots
                    .OrderByDescending(snapshot => snapshot.CapturedAtUtc)
                    .FirstOrDefault();

                if (latestSnapshot is null)
                {
                    return null;
                }

                return new TopMoverDto
                {
                    Symbol = stock.Symbol,
                    CompanyName = stock.CompanyName,
                    CurrentPrice = latestSnapshot.CurrentPrice,
                    PreviousClose = latestSnapshot.PreviousClose,
                    ChangePercent = latestSnapshot.ChangePercent,
                    CapturedAtUtc = latestSnapshot.CapturedAtUtc
                };
            })
            .Where(dto => dto is not null)
            .Select(dto => dto!)
            .OrderByDescending(dto => dto.ChangePercent)
            .Take(limit)
            .ToList();
    }
}
