using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KdxReport.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleToTripReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "trip_reports",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "title",
                table: "trip_reports");
        }
    }
}
