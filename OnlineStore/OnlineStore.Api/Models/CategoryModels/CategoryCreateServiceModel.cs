namespace OnlineStore.Api.Models.CategoryModels
{
    using OnlineStore.Common.AutoMapping;
    using OnlineStore.Models;
    using OnlineStore.Models.Common;
    using System.ComponentModel.DataAnnotations;

    public class CategoryCreateServiceModel : IMapWith<Category>
    {
        [Required]
        [MinLength(ModelConstants.CategoryNameMinLength)]
        public string Name { get; set; }

        public byte[] Thumbnail { get; set; }
    }
}
