namespace OnlineStore.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Holds the current state of the ordered product
    /// </summary>
    public class ProductState
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int OrderId { get; set; }

        public Order Order { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int OrderedAmount { get; set; }
    }
}
