namespace OnlineStore.Models
{
    using Common;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(ModelConstants.CategoryNameMinLength)]
        public string Name { get; set; }

        public byte[] Thumbnail { get; set; }
        
        public IEnumerable<Product> Products { get; set; } = new HashSet<Product>();
    }
}
