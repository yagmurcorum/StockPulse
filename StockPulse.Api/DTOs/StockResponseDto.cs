namespace StockPulse.Api.DTOs;

public class StockResponseDto
{
    public int Id { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public string? CompanyName { get; set; }

    public string? Exchange { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public PriceSnapshotResponseDto? LatestSnapshot { get; set; }
}
