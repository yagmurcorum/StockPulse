namespace StockPulse.Api.Models; 

public class Stock
{
    public int Id { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public string? CompanyName { get; set; }

    public string? Exchange { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<PriceSnapshot> PriceSnapshots { get; set; } = new List<PriceSnapshot>();
}
