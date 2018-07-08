namespace OnlineStore.Models
{
    using Common;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Product
    {
        [Key]
        public int Id { get; set; }

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

        public Category Category { get; set; }

        public IEnumerable<Picture> Pictures { get; set; } = new HashSet<Picture>();
    }
}