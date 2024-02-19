#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Application;

public partial class AddVideos : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.AddColumn<string>(
                name: "MobileVideoPath",
                schema: "Catalog",
                table: "Tours",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoPath",
                schema: "Catalog",
                table: "Tours",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileVideoPath",
                schema: "Catalog",
                table: "Properties",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoPath",
                schema: "Catalog",
                table: "Properties",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropColumn(
                name: "MobileVideoPath",
                schema: "Catalog",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "VideoPath",
                schema: "Catalog",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "MobileVideoPath",
                schema: "Catalog",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "VideoPath",
                schema: "Catalog",
                table: "Properties");
        }
}