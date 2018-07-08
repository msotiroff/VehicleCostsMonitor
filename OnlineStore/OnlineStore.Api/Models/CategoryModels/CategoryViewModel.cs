namespace OnlineStore.Api.Models.CategoryModels
{
    using OnlineStore.Common.AutoMapping;
    using OnlineStore.Models;
    using OnlineStore.Api.Models.ProductModels;
    using System.Collections.Generic;
    using AutoMapper;
    using System;

    public class CategoryViewModel : IMapWith<Category>, IHaveCustomMapping
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string Thumbnail { get; set; }

        public IEnumerable<ProductViewModel> Products { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<Category, CategoryViewModel>()
                .ForMember(dest => dest.Thumbnail, 
                    opt => opt.MapFrom(t => $"data:image/gif;base64,{Convert.ToBase64String(t.Thumbnail)}"));
        }
    }
}
