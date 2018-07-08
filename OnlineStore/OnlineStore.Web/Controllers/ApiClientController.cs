namespace OnlineStore.Web.Controllers
{
    using System;
    using System.Net.Http;

    public abstract class ApiClientController : BaseController
    {
        private const string apiBaseUrl = "http://localhost:49949";
        
        public ApiClientController()
        {
            this.ConfigureHttpClient();
        }
        
        protected HttpClient HttpClient { get; private set; }

        private void ConfigureHttpClient()
        {
            this.HttpClient = new HttpClient();
            HttpClient.BaseAddress = new Uri(apiBaseUrl);
            HttpClient.DefaultRequestHeaders.Clear();
        }
    }
}
