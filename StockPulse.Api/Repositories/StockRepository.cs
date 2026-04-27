using Microsoft.EntityFrameworkCore;
using StockPulse.Api.Data;
using StockPulse.Api.Models;



namespace StockPulse.Api.Repositories;

// Repository Pattern: Database access is abstracted behind this repository to keep services independent from EF Core details.
public class StockRepository : IStockRepository
{
    private readonly AppDbContext _context;

    public StockRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Stock>> GetAllAsync()
    {
        return await _context.Stocks
            .Include(stock => stock.PriceSnapshots)
            .ToListAsync();
    }

    public async Task<Stock?> GetBySymbolAsync(string symbol)
    {
        return await _context.Stocks
            .Include(stock => stock.PriceSnapshots)
            .FirstOrDefaultAsync(stock => stock.Symbol == symbol.ToUpper());
    }

    public async Task<Stock?> GetByIdAsync(int id)
    {
        return await _context.Stocks
            .Include(stock => stock.PriceSnapshots)
            .FirstOrDefaultAsync(stock => stock.Id == id);
    }

    public async Task AddAsync(Stock stock)
    {
        await _context.Stocks.AddAsync(stock);
    }

    public Task DeleteAsync(Stock stock)
    {
        _context.Stocks.Remove(stock);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
