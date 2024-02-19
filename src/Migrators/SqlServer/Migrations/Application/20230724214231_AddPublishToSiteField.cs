#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Application;

public partial class AddPublishToSiteField : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.AddColumn<bool>(
                name: "PublishToSite",
                schema: "Catalog",
                table: "Tours",
                type: "bit",
                nullable: true);
        }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropColumn(
                name: "PublishToSite",
                schema: "Catalog",
                table: "Tours");
        }
}