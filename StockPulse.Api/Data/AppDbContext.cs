using Microsoft.EntityFrameworkCore;
using StockPulse.Api.Models;


namespace StockPulse.Api.Data; 

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<PriceSnapshot> PriceSnapshots => Set<PriceSnapshot>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Stock>()
            .HasIndex(stock => stock.Symbol)
            .IsUnique();

        modelBuilder.Entity<Stock>()
            .Property(stock => stock.Symbol)
            .HasMaxLength(20)
            .IsRequired();

        modelBuilder.Entity<Stock>()
            .Property(stock => stock.CompanyName)
            .HasMaxLength(200);

        modelBuilder.Entity<Stock>()
            .Property(stock => stock.Exchange)
            .HasMaxLength(100);

        modelBuilder.Entity<Stock>()
            .HasMany(stock => stock.PriceSnapshots)
            .WithOne(snapshot => snapshot.Stock)
            .HasForeignKey(snapshot => snapshot.StockId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
