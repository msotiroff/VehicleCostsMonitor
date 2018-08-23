namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Caching.Distributed;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Web.Areas.Vehicle.Models.Enums;
    using VehicleCostsMonitor.Web.Infrastructure.Extensions;
    using static WebConstants;

    public abstract class BaseEntryController : BaseVehicleController
    {
        private const string PricingTypesCacheKey = "_PricingTypesStoredInCache";
        private const string CurrenciesCacheKey = "_CurrenciesStoredInCache";
        
        private readonly ICurrencyService currencyService;

        public BaseEntryController(
            IDistributedCache cache,
            ICurrencyService currencyService)
        {
            this.Cache = cache;
            this.currencyService = currencyService;
        }

        protected IDistributedCache Cache { get; }

        protected async Task<IEnumerable<SelectListItem>> GetAllCurrenciesAsync()
        {
            IEnumerable<SelectListItem> list;

            var listFromCache = await this.Cache.GetStringAsync(CurrenciesCacheKey);
            if (listFromCache == null)
            {
                var currencyList = await this.currencyService.GetAsync();
                list = currencyList.Select(x => new SelectListItem(x.ToString(), x.Id.ToString()));
                var expiration = TimeSpan.FromDays(StaticElementsCacheExpirationInDays);

                await this.Cache.SetSerializableObject(CurrenciesCacheKey, list, expiration);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<SelectListItem>>(listFromCache);
            }

            return list;
        }

        protected async Task<IEnumerable<SelectListItem>> GetAllPricingTypesAsync()
        {
            IEnumerable<SelectListItem> list;

            var listFromCache = await this.Cache.GetStringAsync(PricingTypesCacheKey);
            if (listFromCache == null)
            {
                list = Enum.GetNames(typeof(PricingType)).Select(pt => new SelectListItem(pt.ToString(), pt.ToString()));
                var expiration = TimeSpan.FromDays(StaticElementsCacheExpirationInDays);

                await this.Cache.SetSerializableObject(PricingTypesCacheKey, list, expiration);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<SelectListItem>>(listFromCache);
            }

            return list;
        }
    }
}