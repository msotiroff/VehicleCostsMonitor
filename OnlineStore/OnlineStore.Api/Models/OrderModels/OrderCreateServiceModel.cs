namespace OnlineStore.Api.Models.OrderModels
{
    using Models.ProductModels;
    using OnlineStore.Models.Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OrderCreateServiceModel
    {
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }
        
        [Required]
        [MinLength(ModelConstants.CustomerNameMinLength)]
        public string CustomerName { get; set; }
        
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string CustomerPhoneNumber { get; set; }
        
        public string UserId { get; set; }
        
        public IEnumerable<ProductStateServiceModel> Products { get; set; }
    }
}
