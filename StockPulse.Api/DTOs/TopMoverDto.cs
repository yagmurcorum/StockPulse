namespace StockPulse.Api.DTOs;

public class TopMoverDto
{
    public string Symbol { get; set; } = string.Empty;

    public string? CompanyName { get; set; }

    public decimal CurrentPrice { get; set; }

    public decimal PreviousClose { get; set; }

    public decimal ChangePercent { get; set; }

    public DateTime CapturedAtUtc { get; set; }

}
