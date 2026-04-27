using System.Text.Json.Serialization;


namespace StockPulse.Api.External;

public class FinnhubQuoteResponse
{
    [JsonPropertyName("c")]
    public decimal CurrentPrice { get; set; }

    [JsonPropertyName("h")]
    public decimal HighPrice { get; set; }

    [JsonPropertyName("l")]
    public decimal LowPrice { get; set; }

    [JsonPropertyName("o")]
    public decimal OpenPrice { get; set; }

    [JsonPropertyName("pc")]
    public decimal PreviousClose { get; set; }
}
