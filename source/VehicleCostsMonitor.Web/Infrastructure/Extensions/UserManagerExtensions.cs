namespace VehicleCostsMonitor.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Identity;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;

    public static class UserManagerExtensions
    {
        public static async Task<IdentityResult> UpdateDisplayCurrencyAsync(this UserManager<User> userManager, User user, int? currencyId)
        {
            if (user.CurrencyId == currencyId)
            {
                var error = new IdentityError
                {
                    Description = "Current currency and new currency are equal!"
                };

                return IdentityResult.Failed(error);
            }

            user.CurrencyId = currencyId;
            var result = await userManager.UpdateAsync(user);

            return result;
        }
    }
}
