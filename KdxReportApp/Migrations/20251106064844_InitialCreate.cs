using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KdxReport.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "equipment",
                columns: table => new
                {
                    equipment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    equipment_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    total_counter = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment", x => x.equipment_id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "company_contacts",
                columns: table => new
                {
                    contact_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    contact_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
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

            migrationBuilder.CreateTable(
                name: "role_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_role_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_users_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trip_reports",
                columns: table => new
                {
                    trip_report_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    contact_id = table.Column<int>(type: "integer", nullable: false),
                    equipment_id = table.Column<int>(type: "integer", nullable: false),
                    trip_start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    trip_end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    submitter = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    companions = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    content = table.Column<string>(type: "text", nullable: false),
                    approval_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "pending"),
                    approved_by = table.Column<int>(type: "integer", nullable: true),
                    approved_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_reports", x => x.trip_report_id);
                    table.ForeignKey(
                        name: "FK_trip_reports_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_reports_company_contacts_contact_id",
                        column: x => x.contact_id,
                        principalTable: "company_contacts",
                        principalColumn: "contact_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_reports_equipment_equipment_id",
                        column: x => x.equipment_id,
                        principalTable: "equipment",
                        principalColumn: "equipment_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_reports_users_approved_by",
                        column: x => x.approved_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "read_statuses",
                columns: table => new
                {
                    read_status_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    trip_report_id = table.Column<int>(type: "integer", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    read_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_read_statuses", x => x.read_status_id);
                    table.ForeignKey(
                        name: "FK_read_statuses_trip_reports_trip_report_id",
                        column: x => x.trip_report_id,
                        principalTable: "trip_reports",
                        principalColumn: "trip_report_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_read_statuses_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "threads",
                columns: table => new
                {
                    thread_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    equipment_id = table.Column<int>(type: "integer", nullable: true),
                    trip_report_id = table.Column<int>(type: "integer", nullable: true),
                    thread_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_threads", x => x.thread_id);
                    table.ForeignKey(
                        name: "FK_threads_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_threads_equipment_equipment_id",
                        column: x => x.equipment_id,
                        principalTable: "equipment",
                        principalColumn: "equipment_id");
                    table.ForeignKey(
                        name: "FK_threads_trip_reports_trip_report_id",
                        column: x => x.trip_report_id,
                        principalTable: "trip_reports",
                        principalColumn: "trip_report_id");
                });

            migrationBuilder.CreateTable(
                name: "attachments",
                columns: table => new
                {
                    attachment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    thread_id = table.Column<int>(type: "integer", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    file_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attachments", x => x.attachment_id);
                    table.ForeignKey(
                        name: "FK_attachments_threads_thread_id",
                        column: x => x.thread_id,
                        principalTable: "threads",
                        principalColumn: "thread_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    thread_id = table.Column<int>(type: "integer", nullable: false),
                    parent_comment_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    comment_content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.comment_id);
                    table.ForeignKey(
                        name: "FK_comments_comments_parent_comment_id",
                        column: x => x.parent_comment_id,
                        principalTable: "comments",
                        principalColumn: "comment_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comments_threads_thread_id",
                        column: x => x.thread_id,
                        principalTable: "threads",
                        principalColumn: "thread_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "role_id", "created_at", "role_name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "User", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_attachments_thread_id",
                table: "attachments",
                column: "thread_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_parent_comment_id",
                table: "comments",
                column: "parent_comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_thread_id",
                table: "comments",
                column: "thread_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_user_id",
                table: "comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_company_contacts_company_id",
                table: "company_contacts",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_read_statuses_trip_report_id",
                table: "read_statuses",
                column: "trip_report_id");

            migrationBuilder.CreateIndex(
                name: "IX_read_statuses_user_id_trip_report_id",
                table: "read_statuses",
                columns: new[] { "user_id", "trip_report_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_users_role_id_user_id",
                table: "role_users",
                columns: new[] { "role_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_users_user_id",
                table: "role_users",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_role_name",
                table: "roles",
                column: "role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_threads_company_id",
                table: "threads",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_threads_equipment_id",
                table: "threads",
                column: "equipment_id");

            migrationBuilder.CreateIndex(
                name: "IX_threads_trip_report_id",
                table: "threads",
                column: "trip_report_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_reports_approved_by",
                table: "trip_reports",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "IX_trip_reports_company_id",
                table: "trip_reports",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_reports_contact_id",
                table: "trip_reports",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_reports_equipment_id",
                table: "trip_reports",
                column: "equipment_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attachments");

            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "read_statuses");

            migrationBuilder.DropTable(
                name: "role_users");

            migrationBuilder.DropTable(
                name: "threads");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "trip_reports");

            migrationBuilder.DropTable(
                name: "company_contacts");

            migrationBuilder.DropTable(
                name: "equipment");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "companies");
        }
    }
}
