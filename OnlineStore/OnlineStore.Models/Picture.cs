namespace OnlineStore.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Picture
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}