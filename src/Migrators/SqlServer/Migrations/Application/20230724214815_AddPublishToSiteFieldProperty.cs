#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Application;

public partial class AddPublishToSiteFieldProperty : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.AddColumn<bool>(
                name: "PublishToSite",
                schema: "Catalog",
                table: "Properties",
                type: "bit",
                nullable: true);
        }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropColumn(
                name: "PublishToSite",
                schema: "Catalog",
                table: "Properties");
        }
}