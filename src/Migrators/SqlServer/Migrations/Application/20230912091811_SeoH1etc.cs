#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Application;

public partial class SeoH1etc : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.AddColumn<string>(
                name: "H1",
                schema: "Catalog",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "H2",
                schema: "Catalog",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlSlug",
                schema: "Catalog",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "H1",
                schema: "Catalog",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "H2",
                schema: "Catalog",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlSlug",
                schema: "Catalog",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);
        }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropColumn(
                name: "H1",
                schema: "Catalog",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "H2",
                schema: "Catalog",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "UrlSlug",
                schema: "Catalog",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "H1",
                schema: "Catalog",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "H2",
                schema: "Catalog",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "UrlSlug",
                schema: "Catalog",
                table: "Properties");
        }
}