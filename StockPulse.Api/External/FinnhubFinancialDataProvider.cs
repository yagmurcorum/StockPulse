using Microsoft.Extensions.Options;


namespace StockPulse.Api.External;

// Strategy Pattern: External financial data access is abstracted so Finnhub can be replaced with another provider later.
public class FinnhubFinancialDataProvider : IFinancialDataProvider
{
    private readonly HttpClient _httpClient;
    private readonly FinnhubOptions _options;

    public FinnhubFinancialDataProvider(HttpClient httpClient, IOptions<FinnhubOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<FinancialQuoteDto> GetQuoteAsync(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be empty.", nameof(symbol));
        }

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException("Finnhub API key is not configured.");
        }

        var normalizedSymbol = symbol.Trim().ToUpperInvariant();

        var response = await _httpClient.GetAsync($"quote?symbol={normalizedSymbol}&token={_options.ApiKey}");

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Finnhub API request failed.");
        }

        var quote = await response.Content.ReadFromJsonAsync<FinnhubQuoteResponse>();

        if (quote is null || quote.CurrentPrice <= 0)
        {
            throw new InvalidOperationException("Finnhub returned an invalid quote response.");
        }

        var changePercent = quote.PreviousClose == 0
            ? 0
            : ((quote.CurrentPrice - quote.PreviousClose) / quote.PreviousClose) * 100;

        return new FinancialQuoteDto
        {
            CurrentPrice = quote.CurrentPrice,
            OpenPrice = quote.OpenPrice,
            HighPrice = quote.HighPrice,
            LowPrice = quote.LowPrice,
            PreviousClose = quote.PreviousClose,
            ChangePercent = Math.Round(changePercent, 2)
        };
    }
}
