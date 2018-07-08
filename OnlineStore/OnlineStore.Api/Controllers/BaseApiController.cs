namespace OnlineStore.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using OnlineStore.Data;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public abstract class BaseApiController : Controller
    {
        protected readonly OnlineStoreDbContext db;

        public BaseApiController(OnlineStoreDbContext db)
        {
            this.db = db;
        }
    }
}