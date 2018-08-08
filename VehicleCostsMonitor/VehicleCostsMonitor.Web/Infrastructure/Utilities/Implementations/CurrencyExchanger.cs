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
        private const string RequestUrl = "convert?q={0}_{1}&compact=ultra";

        private IDictionary<string, decimal> rates;
        private readonly string outputCurrency;
        private readonly HttpClient client;

        public CurrencyExchanger(HttpClient client)
        {
            this.client = this.InitializeHttpClient(client);
            this.rates = new Dictionary<string, decimal>();
        }

        public decimal Convert(string inputCurrency, decimal amount, string outputCurrency)
        {
            var rate = this.GetRate(inputCurrency, outputCurrency);

            var coef = amount * rate;

            return coef;
        }

        public decimal GetRate(string inputCurrency, string outputCurrency)
        {
            if (this.rates.ContainsKey(inputCurrency))
            {
                return this.rates[inputCurrency];
            }

            var exchangeUrl = string.Format(RequestUrl, inputCurrency, outputCurrency);
            string response = string.Empty;

            Task.Run(async () =>
            {
                response = await this.client.GetStringAsync(exchangeUrl);
            })
            .GetAwaiter()
            .GetResult();

            var jsonObject = JObject.Parse(response);

            var rate = jsonObject.GetValue($"{inputCurrency}_{outputCurrency}").ToObject<decimal>();

            this.rates[inputCurrency] = rate;

            return rate;
        }

        private HttpClient InitializeHttpClient(HttpClient client)
        {
            client.BaseAddress = new Uri(WebConstants.CurrencyConverterApiBaseUri);
            client.DefaultRequestHeaders.Clear();

            return client;
        }
    }
}
