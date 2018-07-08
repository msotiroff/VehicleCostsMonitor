namespace OnlineStore.Web.Models.OrderViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OrderCreateViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string CustomerName { get; set; }

        [Phone]
        [Required]
        [Display(Name = "Phone Number")]
        public string CustomerPhoneNumber { get; set; }

        public IEnumerable<string> Products { get; set; }

        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
    }
}
