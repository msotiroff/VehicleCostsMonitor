using OnlineStore.Common.AutoMapping;
using OnlineStore.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Api.Models.PictureModels
{
    public class PictureCreateServiceModel : IMapWith<Picture>
    {
        [Required]
        public string Path { get; set; }
        
        [Required]
        public int ProductId { get; set; }
    }
}
