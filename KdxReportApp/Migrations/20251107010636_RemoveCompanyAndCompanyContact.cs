using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KdxReport.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCompanyAndCompanyContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_threads_companies_company_id",
                table: "threads");

            migrationBuilder.DropForeignKey(
                name: "FK_trip_reports_companies_company_id",
                table: "trip_reports");

            migrationBuilder.DropForeignKey(
                name: "FK_trip_reports_company_contacts_contact_id",
                table: "trip_reports");

            migrationBuilder.DropTable(
                name: "company_contacts");

            migrationBuilder.DropTable(
                name: "companies");

            migrationBuilder.DropIndex(
                name: "IX_trip_reports_company_id",
                table: "trip_reports");

            migrationBuilder.DropIndex(
                name: "IX_trip_reports_contact_id",
                table: "trip_reports");

            migrationBuilder.DropIndex(
                name: "IX_threads_company_id",
                table: "threads");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "trip_reports");

            migrationBuilder.DropColumn(
                name: "contact_id",
                table: "trip_reports");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "threads");

            migrationBuilder.AddColumn<string>(
                name: "company_cd",
                table: "trip_reports",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "customer_cd",
                table: "trip_reports",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "staff_cd",
                table: "trip_reports",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "company_cd",
                table: "threads",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "company_cd",
                table: "trip_reports");

            migrationBuilder.DropColumn(
                name: "customer_cd",
                table: "trip_reports");

            migrationBuilder.DropColumn(
                name: "staff_cd",
                table: "trip_reports");

            migrationBuilder.DropColumn(
                name: "company_cd",
                table: "threads");

            migrationBuilder.AddColumn<int>(
                name: "company_id",
                table: "trip_reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "contact_id",
                table: "trip_reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "company_id",
                table: "threads",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    company_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    company_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.company_id);
                });

            migrationBuilder.CreateTable(
                name: "company_contacts",
                columns: table => new
                {
                    contact_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    contact_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_contacts", x => x.contact_id);
                    table.ForeignKey(
                        name: "FK_company_contacts_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trip_reports_company_id",
                table: "trip_reports",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_reports_contact_id",
                table: "trip_reports",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "IX_threads_company_id",
                table: "threads",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_company_contacts_company_id",
                table: "company_contacts",
                column: "company_id");

            migrationBuilder.AddForeignKey(
                name: "FK_threads_companies_company_id",
                table: "threads",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_trip_reports_companies_company_id",
                table: "trip_reports",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_trip_reports_company_contacts_contact_id",
                table: "trip_reports",
                column: "contact_id",
                principalTable: "company_contacts",
                principalColumn: "contact_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
