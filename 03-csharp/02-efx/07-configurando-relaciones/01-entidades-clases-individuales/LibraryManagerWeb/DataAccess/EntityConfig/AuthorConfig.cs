using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace LibraryManagerWeb.DataAccess.EntityConfig
{
    public class AuthorConfig : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> authorBuilder)
        {
            authorBuilder.ToTable("Authors", t => t.HasComment("Tabla para almacenar los autores que tienen libros en la biblioteca"))
                .HasMany(p => p.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(p => p.AuthorUrl)
                .HasPrincipalKey(p => p.AuthorUrl)
                .IsRequired(true);

            authorBuilder.Property(p => p.DisplayName)
                .HasComputedColumnSql("Name + ' ' + LastName", stored: true);
        }
    }
}