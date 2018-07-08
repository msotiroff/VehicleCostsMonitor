namespace OnlineStore.Web.Infrastructure.Extensions
{
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class HttpClientExtensions
    {
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var dataAsString = await content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<T>(dataAsString);

            return obj;
        }
    }
}
