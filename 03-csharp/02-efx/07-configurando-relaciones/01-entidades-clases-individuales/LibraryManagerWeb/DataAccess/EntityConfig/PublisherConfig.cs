using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace LibraryManagerWeb.DataAccess.EntityConfig
{
    public class PublisherConfig : IEntityTypeConfiguration<Publisher>
    {
        public void Configure(EntityTypeBuilder<Publisher> publisherBuilder)
        {
            publisherBuilder
                .Property(p => p.Name).HasColumnName("PublisherName");
        }
    }
}