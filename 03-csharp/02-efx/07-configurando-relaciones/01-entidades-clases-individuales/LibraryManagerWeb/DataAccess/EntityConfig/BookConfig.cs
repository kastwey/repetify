using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagerWeb.DataAccess.EntityConfig
{
    public class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> bookBuilder)
        {
            bookBuilder
                .Ignore(b => b.LoadedDate)
                .Property(p => p.Sinopsis).HasMaxLength(300);

            bookBuilder
                .Property(p => p.Title)
                .UseCollation("SQL_Latin1_General_CP1_CI_AI");

            bookBuilder.Property(b => b.BookId)
                .ValueGeneratedOnAdd();
        }
    }
}