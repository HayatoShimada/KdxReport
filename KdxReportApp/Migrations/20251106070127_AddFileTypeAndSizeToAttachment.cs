using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KdxReport.Migrations
{
    /// <inheritdoc />
    public partial class AddFileTypeAndSizeToAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "file_size",
                table: "attachments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "file_type",
                table: "attachments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "file_size",
                table: "attachments");

            migrationBuilder.DropColumn(
                name: "file_type",
                table: "attachments");
        }
    }
}
