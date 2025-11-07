using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KdxReport.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffSerialNoToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "staff_serial_no",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "staff_serial_no",
                table: "users");
        }
    }
}
