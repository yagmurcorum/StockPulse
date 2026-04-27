using StockPulse.Api.Models;


namespace StockPulse.Api.Repositories;

public interface IStockRepository
{
    Task<List<Stock>> GetAllAsync();

    Task<Stock?> GetBySymbolAsync(string symbol);

    Task<Stock?> GetByIdAsync(int id);

    Task AddAsync(Stock stock);

    Task DeleteAsync(Stock stock);

    Task SaveChangesAsync();
}
