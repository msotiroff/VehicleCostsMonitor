namespace OnlineStore.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using OnlineStore.Services.Models.FeedbackModels;
    using OnlineStore.Common.Notifications;
    using OnlineStore.Models;
    using OnlineStore.Web.Infrastructure.Extensions;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using static WebConstants;
    using OnlineStore.Services.Interfaces;

    public class FeedbackController : BaseController
    {
        private readonly UserManager<User> userManager;
        private readonly IFeedbackService feedbackService;

        public FeedbackController(UserManager<User> userManager, IFeedbackService feedbackService)
        {
            this.userManager = userManager;
            this.feedbackService = feedbackService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Add() => View();

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(FeedbackCreateServiceModel model)
        {
            model.SenderId = this.userManager.GetUserId(User);
            await this.feedbackService.CreateAsync(model);

            this.ShowNotification(NotificationMessages.FeedbackSentSuccessfull, NotificationType.Info);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [Authorize(Roles = AdministratorRole)]
        public async Task<IActionResult> All()
        {
            var model = await this.feedbackService.GetAllAsync();

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = AdministratorRole)]
        public async Task<IActionResult> Details(int id)
        {
            var model = await this.feedbackService.GetAsync(id);
            model.SenderEmail = this.userManager.GetUserName(User);

            return View(model);
        }
    }
}