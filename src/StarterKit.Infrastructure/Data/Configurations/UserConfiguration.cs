using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarterKit.Domain.Entities;

namespace StarterKit.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id).ValueGeneratedOnAdd();

        builder.Property(u => u.Name).IsRequired().HasColumnName("name");
        builder.Property(u => u.Email).IsRequired().HasColumnName("email");
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Password).IsRequired().HasColumnName("password");
        builder.Property(u => u.Role).HasConversion<string>().HasColumnName("role");

        // Keep previous fix
        builder.Property(u => u.IsEmailVerified)
            .HasColumnName("is_email_verified")
            .HasConversion<int>(); 

        builder.Property(u => u.CreatedAt).HasColumnName("created_at");
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");
    }
}