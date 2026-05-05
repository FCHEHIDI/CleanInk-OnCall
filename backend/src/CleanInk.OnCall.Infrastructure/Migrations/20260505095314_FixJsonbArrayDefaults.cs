using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanInk.OnCall.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixJsonbArrayDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Fix JSONB array columns whose DB default was incorrectly set to '{}' (object)
            // instead of '[]' (array). Rows already fixed via direct SQL; this corrects the
            // column-level DEFAULT so future INSERT statements without explicit values are safe.
            migrationBuilder.AlterColumn<string>(
                name: "contact_points",
                table: "patients",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldDefaultValueSql: "'{}'::jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "identifiers",
                table: "patients",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldDefaultValueSql: "'{}'::jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "names",
                table: "patients",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldDefaultValueSql: "'{}'::jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "contact_points",
                table: "patients",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{}'::jsonb",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldDefaultValueSql: "'[]'::jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "identifiers",
                table: "patients",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{}'::jsonb",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldDefaultValueSql: "'[]'::jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "names",
                table: "patients",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{}'::jsonb",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldDefaultValueSql: "'[]'::jsonb");
        }
    }
}
