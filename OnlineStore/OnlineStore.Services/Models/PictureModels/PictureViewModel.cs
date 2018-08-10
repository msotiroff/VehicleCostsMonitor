namespace OnlineStore.Services.Models.PictureModels
{
    using OnlineStore.Common.AutoMapping;
    using OnlineStore.Models;

    public class PictureViewModel : IMapWith<Picture>
    {
        public int Id { get; set; }
        
        public string Path { get; set; }
        
        public int ProductId { get; set; }
    }
}
