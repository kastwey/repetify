using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace LibraryManagerWeb.DataAccess.EntityConfig
{
    public class MagazineConfig : IEntityTypeConfiguration<Magazine>
    {
        public void Configure(EntityTypeBuilder<Magazine> magazineBuilder)
        {
            magazineBuilder
                            .HasOne(m => m.Category)
                            .WithMany(c => c.Magazines)
                            .IsRequired(true);

            magazineBuilder.Property(p => p.Price)
                .HasPrecision(10, 2);

            magazineBuilder.Property(p => p.Title)
                .HasColumnName("MagazineTitle")
                .HasMaxLength(150)
                .UseCollation("SQL_Latin1_General_CP1_CI_AI");

            magazineBuilder.Property(p => p.Date)
                .HasComment("Campo para indicar la fecha de publicación");

            magazineBuilder.Ignore(p => p.LoadedDate);

            magazineBuilder.HasKey(p => p.MagazineId);

            magazineBuilder.Property<DateTime>("CreationOn")
                .HasDefaultValueSql("getutcdate()");

            magazineBuilder.HasIndex(p => new { p.Title, p.CategoryId })
                .IsUnique(true)
                .HasDatabaseName("UX_Magazine_TitleCategory");
        }
    }
}