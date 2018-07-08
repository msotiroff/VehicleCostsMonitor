namespace OnlineStore.Api.Models.FeedbackModels
{
    using AutoMapper;
    using Common.AutoMapping;
    using OnlineStore.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class FeedbackDetailsServiceModel : IMapWith<Feedback>, IHaveCustomMapping
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
        [Display(Name = "Date Sent")]
        public DateTime DateSent { get; set; }
        
        [Display(Name = "Sender email")]
        public string SenderEmail { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Feedback, FeedbackDetailsServiceModel>()
                .ForMember(dest => dest.SenderEmail, opt => opt.MapFrom(src => src.Sender.Email));
        }
    }
}
