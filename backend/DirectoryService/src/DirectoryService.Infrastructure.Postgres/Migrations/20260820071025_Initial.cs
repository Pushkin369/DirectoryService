using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    slug = table.Column<string>(type: "character varying(124)", maxLength: 124, nullable: false),
                    tree_path = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_department", x => x.id);
                    table.ForeignKey(
                        name: "fk_department_parent",
                        column: x => x.parent_id,
                        principalTable: "department",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "location",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    building = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    city = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    country = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    district = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    floor = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    region = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    room = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_location", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "position",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_position", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "department_location",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_department_location", x => x.id);
                    table.ForeignKey(
                        name: "fk_department_location_department",
                        column: x => x.department_id,
                        principalTable: "department",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_department_location_location",
                        column: x => x.location_id,
                        principalTable: "location",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "department_position",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_department_position", x => x.id);
                    table.ForeignKey(
                        name: "fk_department_position_department",
                        column: x => x.department_id,
                        principalTable: "department",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_department_position_position",
                        column: x => x.position_id,
                        principalTable: "position",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_department_parent_id",
                table: "department",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_department_tree_path",
                table: "department",
                column: "tree_path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_department_location_location_id",
                table: "department_location",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ux_department_location_pair",
                table: "department_location",
                columns: new[] { "department_id", "location_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_department_position_position_id",
                table: "department_position",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "ux_department_position_pair",
                table: "department_position",
                columns: new[] { "department_id", "position_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "department_location");

            migrationBuilder.DropTable(
                name: "department_position");

            migrationBuilder.DropTable(
                name: "location");

            migrationBuilder.DropTable(
                name: "department");

            migrationBuilder.DropTable(
                name: "position");
        }
    }
}
