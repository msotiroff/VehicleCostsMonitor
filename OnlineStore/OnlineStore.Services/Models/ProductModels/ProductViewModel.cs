namespace OnlineStore.Services.Models.ProductModels
{
    using OnlineStore.Common.AutoMapping;
    using OnlineStore.Models;
    using OnlineStore.Services.Models.PictureModels;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ProductViewModel : IMapWith<Product>
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public decimal Price { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        public IEnumerable<PictureViewModel> Pictures { get; set; }
    }
}
