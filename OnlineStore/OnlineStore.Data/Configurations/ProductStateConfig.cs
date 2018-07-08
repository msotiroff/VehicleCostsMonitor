namespace OnlineStore.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OnlineStore.Models;

    public class ProductStateConfig : IEntityTypeConfiguration<ProductState>
    {
        public void Configure(EntityTypeBuilder<ProductState> builder)
        {
            builder
                .HasOne(ps => ps.Order)
                .WithMany(o => o.Products)
                .HasForeignKey(ps => ps.OrderId);
        }
    }
}
