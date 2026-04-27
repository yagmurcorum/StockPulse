using StockPulse.Api.DTOs;
using StockPulse.Api.External;
using StockPulse.Api.Models;
using StockPulse.Api.Repositories;

namespace StockPulse.Api.Services;

public class StockService : IStockService
{
    private readonly IStockRepository _stockRepository;
    private readonly IFinancialDataProvider _financialDataProvider;

    public StockService(
        IStockRepository stockRepository,
        IFinancialDataProvider financialDataProvider)
    {
        _stockRepository = stockRepository;
        _financialDataProvider = financialDataProvider;
    }

    public async Task<List<StockResponseDto>> GetAllAsync()
    {
        var stocks = await _stockRepository.GetAllAsync();

        return stocks
            .Select(MapToStockResponseDto)
            .ToList();
    }

    public async Task<StockResponseDto?> GetBySymbolAsync(string symbol)
    {
        var stock = await _stockRepository.GetBySymbolAsync(symbol);

        return stock is null ? null : MapToStockResponseDto(stock);
    }

    public async Task<StockResponseDto> TrackStockAsync(string symbol)
    {
        var normalizedSymbol = NormalizeSymbol(symbol);

        var existingStock = await _stockRepository.GetBySymbolAsync(normalizedSymbol);

        if (existingStock is not null)
        {
            return await RefreshStockAsync(normalizedSymbol);
        }

        var quote = await _financialDataProvider.GetQuoteAsync(normalizedSymbol);

        var stock = new Stock
        {
            Symbol = normalizedSymbol,
            CompanyName = normalizedSymbol,
            Exchange = "Unknown",
            CreatedAtUtc = DateTime.UtcNow
        };

        stock.PriceSnapshots.Add(CreateSnapshot(quote));

        await _stockRepository.AddAsync(stock);
        await _stockRepository.SaveChangesAsync();

        return MapToStockResponseDto(stock);
    }

    public async Task<StockResponseDto> RefreshStockAsync(string symbol)
    {
        var normalizedSymbol = NormalizeSymbol(symbol);

        var stock = await _stockRepository.GetBySymbolAsync(normalizedSymbol);

        if (stock is null)
        {
            throw new InvalidOperationException("Stock was not found.");
        }

        var quote = await _financialDataProvider.GetQuoteAsync(normalizedSymbol);

        stock.PriceSnapshots.Add(CreateSnapshot(quote));

        await _stockRepository.SaveChangesAsync();

        return MapToStockResponseDto(stock);
    }

    public async Task<bool> DeleteStockAsync(string symbol)
    {
        var normalizedSymbol = NormalizeSymbol(symbol);

        var stock = await _stockRepository.GetBySymbolAsync(normalizedSymbol);

        if (stock is null)
        {
            return false;
        }

        await _stockRepository.DeleteAsync(stock);
        await _stockRepository.SaveChangesAsync();

        return true;
    }

    private static PriceSnapshot CreateSnapshot(FinancialQuoteDto quote)
    {
        return new PriceSnapshot
        {
            CapturedAtUtc = DateTime.UtcNow,
            CurrentPrice = quote.CurrentPrice,
            OpenPrice = quote.OpenPrice,
            HighPrice = quote.HighPrice,
            LowPrice = quote.LowPrice,
            PreviousClose = quote.PreviousClose,
            ChangePercent = quote.ChangePercent
        };
    }

    private static StockResponseDto MapToStockResponseDto(Stock stock)
    {
        var latestSnapshot = stock.PriceSnapshots
            .OrderByDescending(snapshot => snapshot.CapturedAtUtc)
            .FirstOrDefault();

        return new StockResponseDto
        {
            Id = stock.Id,
            Symbol = stock.Symbol,
            CompanyName = stock.CompanyName,
            Exchange = stock.Exchange,
            CreatedAtUtc = stock.CreatedAtUtc,
            LatestSnapshot = latestSnapshot is null
                ? null
                : new PriceSnapshotResponseDto
                {
                    Id = latestSnapshot.Id,
                    CapturedAtUtc = latestSnapshot.CapturedAtUtc,
                    CurrentPrice = latestSnapshot.CurrentPrice,
                    OpenPrice = latestSnapshot.OpenPrice,
                    HighPrice = latestSnapshot.HighPrice,
                    LowPrice = latestSnapshot.LowPrice,
                    PreviousClose = latestSnapshot.PreviousClose,
                    ChangePercent = latestSnapshot.ChangePercent
                }
        };
    }

    private static string NormalizeSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be empty.", nameof(symbol));
        }

        return symbol.Trim().ToUpperInvariant();
    }
}
