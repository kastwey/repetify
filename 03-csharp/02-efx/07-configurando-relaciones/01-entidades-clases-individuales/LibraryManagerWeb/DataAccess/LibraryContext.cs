using Microsoft.EntityFrameworkCore;

namespace LibraryManagerWeb.DataAccess
{
    public class LibraryContext : DbContext
    {
        public DbSet<BookFile> BookFiles { get; set; }
        public DbSet<AuditEntry> AuditEntries { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Magazine> Magazines { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }
    }
}
