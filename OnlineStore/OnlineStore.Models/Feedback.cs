namespace OnlineStore.Models
{
    using OnlineStore.Models.Common;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Feedback
    {
        public Feedback()
        {
            this.DateSent = DateTime.UtcNow;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(ModelConstants.FeedbackTitleMinLength)]
        [MaxLength(ModelConstants.FeedbackTitleMaxLength)]
        public string Title { get; set; }

        [Required]
        [MinLength(ModelConstants.FeedbackContentMinLength)]
        public string Content { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateSent { get; set; }

        [Required]
        public string SenderId { get; set; }

        public User Sender { get; set; }
    }
}
