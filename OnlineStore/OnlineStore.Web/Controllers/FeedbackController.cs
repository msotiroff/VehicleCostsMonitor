namespace OnlineStore.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using OnlineStore.Api.Models.FeedbackModels;
    using OnlineStore.Common.Notifications;
    using OnlineStore.Models;
    using OnlineStore.Web.Infrastructure.Extensions;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using static WebConstants;

    public class FeedbackController : ApiClientController
    {
        private const string RequestUri = "api/Feedbacks/";

        private readonly UserManager<User> userManager;

        public FeedbackController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Add() => View();

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(FeedbackCreateServiceModel model)
        {
            model.SenderId = this.userManager.GetUserId(User);

            var postTask = await this.HttpClient.PostAsJsonAsync(RequestUri, model);
            if (!postTask.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            this.ShowNotification(NotificationMessages.FeedbackSentSuccessfull, NotificationType.Info);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [Authorize(Roles = AdministratorRole)]
        public async Task<IActionResult> All()
        {
            var response = await this.HttpClient.GetAsync(RequestUri);

            var model = await response.Content.ReadAsJsonAsync<IEnumerable<FeedbackListingViewModel>>();

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = AdministratorRole)]
        public async Task<IActionResult> Details(int id)
        {
            var response = await this.HttpClient.GetAsync(RequestUri + id);

            var model = await response.Content.ReadAsJsonAsync<FeedbackDetailsServiceModel>();
            model.SenderEmail = this.userManager.GetUserName(User);

            return View(model);
        }
    }
}