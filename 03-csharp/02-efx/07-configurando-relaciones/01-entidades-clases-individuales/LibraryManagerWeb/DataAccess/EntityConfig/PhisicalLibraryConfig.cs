using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace LibraryManagerWeb.DataAccess.EntityConfig
{
    public class PhisicalLibraryConfig : IEntityTypeConfiguration<PhisicalLibrary>
    {
        public void Configure(EntityTypeBuilder<PhisicalLibrary> phisicalLibraryConfigBuilder)
        {
            phisicalLibraryConfigBuilder
                .ToTable("PhisicalLibraries", t => t.HasComment("Tabla para almacenar la libreria fisica donde se encuentra el libro"));
        }
    }
}