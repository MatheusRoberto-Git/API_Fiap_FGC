using FGC.Domain.GameManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FGC.Infrastructure.Data.Configurations
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Games");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.Id)
                .HasColumnName("Id")
                .IsRequired();

            builder.Property(g => g.Title)
                .HasColumnName("Title")
                .HasColumnType("NVARCHAR(200)")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(g => g.Description)
                .HasColumnName("Description")
                .HasColumnType("NVARCHAR(2000)")
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(g => g.Price)
                .HasColumnName("Price")
                .HasColumnType("DECIMAL(18,2)")
                .IsRequired();

            builder.Property(g => g.Category)
                .HasConversion<int>()
                .HasColumnName("Category")
                .HasColumnType("INT")
                .IsRequired();

            builder.Property(g => g.Developer)
                .HasColumnName("Developer")
                .HasColumnType("NVARCHAR(200)")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(g => g.Publisher)
                .HasColumnName("Publisher")
                .HasColumnType("NVARCHAR(200)")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(g => g.ReleaseDate)
                .HasColumnName("ReleaseDate")
                .HasColumnType("DATETIME2")
                .IsRequired();

            builder.Property(g => g.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("DATETIME2")
                .IsRequired();

            builder.Property(g => g.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .HasColumnType("DATETIME2")
                .IsRequired(false);

            builder.Property(g => g.IsActive)
                .HasColumnName("IsActive")
                .HasColumnType("BIT")
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(g => g.Rating)
                .HasColumnName("Rating")
                .HasColumnType("FLOAT")
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(g => g.TotalSales)
                .HasColumnName("TotalSales")
                .HasColumnType("INT")
                .IsRequired()
                .HasDefaultValue(0);

            // Indexes
            builder.HasIndex(g => g.Title)
                .HasDatabaseName("IX_Games_Title");

            builder.HasIndex(g => g.Category)
                .HasDatabaseName("IX_Games_Category");

            builder.HasIndex(g => g.IsActive)
                .HasDatabaseName("IX_Games_IsActive");

            builder.HasIndex(g => g.Price)
                .HasDatabaseName("IX_Games_Price");

            builder.Ignore(g => g.DomainEvents);
        }
    }
}
