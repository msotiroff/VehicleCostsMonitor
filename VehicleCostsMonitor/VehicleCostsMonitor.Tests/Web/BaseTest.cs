namespace VehicleCostsMonitor.Tests.Web
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public abstract class BaseTest
    {
        protected BaseTest()
        {
            TestSetup.InitializeMapper();
        }

        protected void AddClaimsPrincipal(Controller controller, string username)
        {
            controller.ControllerContext = this.SetControllerContext(username);
        }

        private ControllerContext SetControllerContext(string username)
        {
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                    new ClaimsIdentity(new Claim[]
                    {
                            new Claim(ClaimTypes.Name, username)
                    }, "someAuthTypeName"))
                }
            };
        }
    }
}
