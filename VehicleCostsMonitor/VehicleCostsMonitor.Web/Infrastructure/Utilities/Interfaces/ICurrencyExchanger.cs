namespace VehicleCostsMonitor.Web.Infrastructure.Utilities.Interfaces
{
    public interface ICurrencyExchanger
    {
        decimal GetRate(string inputCurrency, string outputCurrency);

        decimal Convert(string inputCurrency, decimal amount, string outputCurrency);
    }
}
