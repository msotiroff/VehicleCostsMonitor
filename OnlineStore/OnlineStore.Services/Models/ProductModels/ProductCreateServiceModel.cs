namespace OnlineStore.Services.Models.ProductModels
{
    using OnlineStore.Common.AutoMapping;
    using OnlineStore.Models;
    using OnlineStore.Models.Common;
    using System.ComponentModel.DataAnnotations;

    public class ProductCreateServiceModel : IMapWith<Product>
    {
        [Required]
        [MinLength(ModelConstants.ProductNameMinLength)]
        public string Name { get; set; }
        
        [Required]
        public string Description { get; set; }

        [Required]
        [Range(typeof(decimal), ModelConstants.ProductPriceMinValueAsString, ModelConstants.ProductPriceMaxValueAsString)]
        public decimal Price { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
    }
}
