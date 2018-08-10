namespace OnlineStore.Services.Models.FeedbackModels
{
    using OnlineStore.Common.AutoMapping;
    using OnlineStore.Models;
    using OnlineStore.Models.Common;
    using System.ComponentModel.DataAnnotations;

    public class FeedbackCreateServiceModel : IMapWith<Feedback>
    {
        [Required]
        [MinLength(ModelConstants.FeedbackTitleMinLength)]
        [MaxLength(ModelConstants.FeedbackTitleMaxLength)]
        public string Title { get; set; }

        [Required]
        [MinLength(ModelConstants.FeedbackContentMinLength)]
        public string Content { get; set; }

        [Required]
        public string SenderId { get; set; }
    }
}
