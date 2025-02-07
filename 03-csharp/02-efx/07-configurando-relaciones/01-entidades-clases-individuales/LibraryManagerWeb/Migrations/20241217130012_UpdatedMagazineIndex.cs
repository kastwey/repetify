using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagerWeb.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedMagazineIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoadedDate",
                table: "Magazines");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationOn",
                table: "Magazines",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getutcdate()");

            migrationBuilder.CreateIndex(
                name: "UX_Magazine_TitleCategory",
                table: "Magazines",
                columns: new[] { "MagazineTitle", "CategoryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Magazine_TitleCategory",
                table: "Magazines");

            migrationBuilder.DropColumn(
                name: "CreationOn",
                table: "Magazines");

            migrationBuilder.AddColumn<DateTime>(
                name: "LoadedDate",
                table: "Magazines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
