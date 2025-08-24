using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobHuntAPI.Migrations
{
	/// <inheritdoc />
	public partial class Initial : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					UserId = table.Column<Guid>(type: "uuid", nullable: false),
					UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					HashedPassword = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Users", x => x.UserId);
				});

			migrationBuilder.CreateTable(
				name: "Applications",
				columns: table => new
				{
					JobApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
					UserId = table.Column<Guid>(type: "uuid", nullable: false),
					JobTitle = table.Column<string>(type: "text", nullable: false),
					Company = table.Column<string>(type: "text", nullable: false),
					AppliedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					ReplyDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					Base64Application = table.Column<string>(type: "text", nullable: false),
					Base64Resume = table.Column<string>(type: "text", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Applications", x => x.JobApplicationId);
					table.ForeignKey(
						name: "FK_Applications_Users_UserId",
						column: x => x.UserId,
						principalTable: "Users",
						principalColumn: "UserId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Applications_UserId",
				table: "Applications",
				column: "UserId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Applications");

			migrationBuilder.DropTable(
				name: "Users");
		}
	}
}
