using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdgarRepo.Migrations
{
    /// <inheritdoc />
    public partial class NetIncomeLosses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frame",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Companies");

            migrationBuilder.CreateTable(
                name: "YearNetIncomeLoss",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Frame = table.Column<string>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    CompanyCik = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearNetIncomeLoss", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YearNetIncomeLoss_Companies_CompanyCik",
                        column: x => x.CompanyCik,
                        principalTable: "Companies",
                        principalColumn: "Cik");
                });

            migrationBuilder.CreateIndex(
                name: "IX_YearNetIncomeLoss_CompanyCik",
                table: "YearNetIncomeLoss",
                column: "CompanyCik");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YearNetIncomeLoss");

            migrationBuilder.AddColumn<string>(
                name: "Frame",
                table: "Companies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "Companies",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
