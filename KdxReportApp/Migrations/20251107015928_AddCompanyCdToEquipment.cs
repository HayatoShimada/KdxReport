using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KdxReport.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyCdToEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "company_cd",
                table: "equipment",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "company_cd",
                table: "equipment");
        }
    }
}
