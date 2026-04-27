namespace StockPulse.Api.External;

public interface IFinancialDataProvider
{
    Task<FinancialQuoteDto> GetQuoteAsync(string symbol);
}
