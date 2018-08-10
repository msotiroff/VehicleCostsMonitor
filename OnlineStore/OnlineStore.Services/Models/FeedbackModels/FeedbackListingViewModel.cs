namespace OnlineStore.Services.Models.FeedbackModels
{
    using AutoMapper;
    using Common.AutoMapping;
    using OnlineStore.Models;
    using System.ComponentModel.DataAnnotations;

    public class FeedbackListingViewModel : IMapWith<Feedback>, IHaveCustomMapping
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }

        [Display(Name = "Sender email")]
        public string SenderEmail { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<Feedback, FeedbackListingViewModel>()
                .ForMember(dest => dest.Content, 
                    opt => opt.MapFrom(src => src.Content.Length > 100 
                        ? src.Content.Substring(0, 100) + "..." 
                        : src.Content));
        }
    }
}
