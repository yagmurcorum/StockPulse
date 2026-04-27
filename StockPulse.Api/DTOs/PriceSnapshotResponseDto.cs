namespace StockPulse.Api.DTOs;

public class PriceSnapshotResponseDto
{
    public int Id { get; set; }

    public DateTime CapturedAtUtc { get; set; }

    public decimal CurrentPrice { get; set; }

    public decimal OpenPrice { get; set; }

    public decimal HighPrice { get; set; }

    public decimal LowPrice { get; set; }

    public decimal PreviousClose { get; set; }

    public decimal ChangePercent { get; set; }
}
