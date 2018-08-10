namespace VehicleCostsMonitor.Web.Infrastructure.Utilities.Interfaces
{
    using System.Threading.Tasks;

    public interface ICurrencyExchanger
    {
        Task<decimal> Convert(string inputCurrency, decimal amount, string outputCurrency);
    }
}
