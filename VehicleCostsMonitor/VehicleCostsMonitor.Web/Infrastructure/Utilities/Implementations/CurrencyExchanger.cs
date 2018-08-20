namespace VehicleCostsMonitor.Web.Infrastructure.Utilities.Implementations
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Web.Infrastructure.Utilities.Interfaces;

    public class CurrencyExchanger : ICurrencyExchanger
    {
        private const string CurrencyConverterApiBaseUri = "https://free.currencyconverterapi.com/api/v6/";
        private const string RequestUrl = "convert?q={0}_{1}&compact=ultra";

        private readonly IDictionary<string, decimal> rates;
        private readonly HttpClient client;

        public CurrencyExchanger(HttpClient client)
        {
            this.client = this.InitializeHttpClient(client);
            this.rates = new Dictionary<string, decimal>();
        }

        public async Task<decimal> Convert(string inputCurrency, decimal amount, string outputCurrency)
        {
            var rate = await this.GetRate(inputCurrency, outputCurrency);

            var convertedAmount = amount * rate;

            return convertedAmount;
        }

        private async Task<decimal> GetRate(string inputCurrency, string outputCurrency)
        {
            if (this.rates.ContainsKey(inputCurrency))
            {
                return this.rates[inputCurrency];
            }

            var exchangeUrl = string.Format(RequestUrl, inputCurrency, outputCurrency);
            string response = await this.client.GetStringAsync(exchangeUrl);
            
            var jsonObject = JObject.Parse(response);

            var rate = jsonObject.GetValue($"{inputCurrency}_{outputCurrency}").ToObject<decimal>();

            this.rates[inputCurrency] = rate;

            return rate;
        }

        private HttpClient InitializeHttpClient(HttpClient client)
        {
            client.BaseAddress = new Uri(CurrencyConverterApiBaseUri);
            client.DefaultRequestHeaders.Clear();

            return client;
        }
    }
}
