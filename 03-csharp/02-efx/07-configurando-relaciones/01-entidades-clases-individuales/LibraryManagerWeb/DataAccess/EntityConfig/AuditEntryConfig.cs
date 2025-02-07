using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagerWeb.DataAccess.EntityConfig
{
    public class AuditEntryConfig : IEntityTypeConfiguration<AuditEntry>
    {
        public void Configure(EntityTypeBuilder<AuditEntry> auditEntryBuilder)
        {
            auditEntryBuilder.Property(p => p.TimeSpent)
                .HasPrecision(20);

            auditEntryBuilder.Property(p => p.IpAddress)
                .IsRequired();

            auditEntryBuilder.Property(p => p.Date)
                .HasDefaultValueSql("getutcdate()");

            auditEntryBuilder.Property<string>("ResearchTicketId").HasMaxLength(20);

            auditEntryBuilder.HasIndex("ResearchTicketId")
                .IsUnique(true)
                .HasDatabaseName("UX_AuditEntry_ReseachTicketId");
        }
    }
}
