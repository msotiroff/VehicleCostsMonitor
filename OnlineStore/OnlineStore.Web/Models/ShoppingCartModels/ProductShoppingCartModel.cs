namespace OnlineStore.Web.Models.ShoppingCartModels
{
    using AutoMapper;
    using OnlineStore.Services.Models.ProductModels;
    using OnlineStore.Common.AutoMapping;
    using System.Linq;

    public class ProductShoppingCartModel : IMapWith<ProductViewModel>, IHaveCustomMapping
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string PicturePath { get; set; }

        public int Amount { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<ProductViewModel, ProductShoppingCartModel>()
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.Description.Length > 50
                        ? src.Description.Substring(0, 50) + "..."
                        : src.Description))
                .ForMember(dest => dest.PicturePath,
                    opt => opt.MapFrom(src => src.Pictures.Any()
                        ? src.Pictures.First().Path
                        : null));
        }
    }
}