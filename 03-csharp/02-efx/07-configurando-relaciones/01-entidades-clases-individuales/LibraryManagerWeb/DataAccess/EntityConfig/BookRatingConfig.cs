using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace LibraryManagerWeb.DataAccess.EntityConfig
{
    public class BookRatingConfig : IEntityTypeConfiguration<BookRating>
    {
        public void Configure(EntityTypeBuilder<BookRating> bookRatingBuilder)
        {
            bookRatingBuilder.ToTable("BookRatings")
                .Property(p => p.Starts).HasColumnType("decimal(3,2)")
                .HasDefaultValue(3);
        }
    }
}