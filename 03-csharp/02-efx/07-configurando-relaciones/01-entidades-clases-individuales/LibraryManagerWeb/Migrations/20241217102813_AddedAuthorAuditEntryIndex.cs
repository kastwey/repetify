using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagerWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddedAuthorAuditEntryIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Authors_Name_LastName",
                table: "Authors",
                columns: new[] { "Name", "LastName" });

            migrationBuilder.CreateIndex(
                name: "UX_AuditEntry_ReseachTicketId",
                table: "AuditEntries",
                column: "ResearchTicketId",
                unique: true,
                filter: "[ResearchTicketId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Authors_Name_LastName",
                table: "Authors");

            migrationBuilder.DropIndex(
                name: "UX_AuditEntry_ReseachTicketId",
                table: "AuditEntries");
        }
    }
}
