using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repetify.Infrastructure.Persistence.EfCore.Migrations
{
	/// <inheritdoc />
	public partial class InitialMigration : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Users", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Decks",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
					UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					OriginalLanguage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					TranslatedLanguage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Decks", x => x.Id);
					table.ForeignKey(
						name: "FK_Decks_Users_UserId",
						column: x => x.UserId,
						principalTable: "Users",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Cards",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					DeckId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					OriginalWord = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
					TranslatedWord = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
					CorrectReviewStreak = table.Column<int>(type: "int", nullable: false),
					NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					PreviousCorrectReview = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Cards", x => x.Id);
					table.ForeignKey(
						name: "FK_Cards_Decks_DeckId",
						column: x => x.DeckId,
						principalTable: "Decks",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Cards_DeckId",
				table: "Cards",
				column: "DeckId");

			migrationBuilder.CreateIndex(
				name: "IX_Decks_UserId",
				table: "Decks",
				column: "UserId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Cards");

			migrationBuilder.DropTable(
				name: "Decks");

			migrationBuilder.DropTable(
				name: "Users");
		}
	}
}
