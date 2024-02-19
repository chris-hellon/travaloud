#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Application;

public partial class ParentTourCategoryId : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentTourCategoryId",
                schema: "Catalog",
                table: "TourCategories",
                type: "uniqueidentifier",
                nullable: true);
        }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropColumn(
                name: "ParentTourCategoryId",
                schema: "Catalog",
                table: "TourCategories");
        }
}