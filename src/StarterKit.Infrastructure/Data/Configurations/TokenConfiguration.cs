using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarterKit.Domain.Entities;

namespace StarterKit.Infrastructure.Data.Configurations;

public class TokenConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.ToTable("tokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.TokenValue).IsRequired().HasColumnName("token");
        builder.Property(t => t.Type).HasConversion<string>().HasColumnName("type");
        builder.Property(t => t.Expires).HasColumnName("expires");

        builder.Property(t => t.Blacklisted)
            .HasColumnName("blacklisted")
            .HasConversion<int>();

        builder.Property(t => t.UserId).HasColumnName("user_id");
        builder.Property(t => t.CreatedAt).HasColumnName("created_at");
        builder.Property(t => t.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(t => t.User)
            .WithMany(u => u.Tokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}