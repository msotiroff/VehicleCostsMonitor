namespace OnlineStore.Web.Infrastructure.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class ValidateReturnUrlAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var returnUrl = context.HttpContext.Request.Query["returnUrl"].ToString();
            
            var controller = context.Controller as ControllerBase;

            var isLocalUrl = controller.Url.IsLocalUrl(returnUrl);

            if (controller == null || !isLocalUrl)
            {
                context.Result = new RedirectResult("/");
            }
        }
    }
}
