namespace OnlineStore.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class FeedbackConfig : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder
                .HasOne(f => f.Sender)
                .WithMany(s => s.SentFeedbacks)
                .HasForeignKey(f => f.SenderId);
        }
    }
}
