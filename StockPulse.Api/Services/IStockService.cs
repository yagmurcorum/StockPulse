using StockPulse.Api.DTOs;


namespace StockPulse.Api.Services;

public interface IStockService
{
    Task<List<StockResponseDto>> GetAllAsync();

    Task<StockResponseDto?> GetBySymbolAsync(string symbol);

    Task<StockResponseDto> TrackStockAsync(string symbol);

    Task<StockResponseDto> RefreshStockAsync(string symbol);

    Task<bool> DeleteStockAsync(string symbol);

}
