using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagerWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddedAlternateKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_AuthorId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Books");

            migrationBuilder.AddColumn<string>(
                name: "AuthorUrl",
                table: "Books",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthorUrl",
                table: "Authors",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Authors_AuthorUrl",
                table: "Authors",
                column: "AuthorUrl");

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorUrl",
                table: "Books",
                column: "AuthorUrl");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorUrl",
                table: "Books",
                column: "AuthorUrl",
                principalTable: "Authors",
                principalColumn: "AuthorUrl",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorUrl",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_AuthorUrl",
                table: "Books");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Authors_AuthorUrl",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "AuthorUrl",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "AuthorUrl",
                table: "Authors");

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
