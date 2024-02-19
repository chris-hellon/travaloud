#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Application;

public partial class PartnerUpdates : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.AlterColumn<string>(
                name: "GoogleMapsPlaceId",
                schema: "Catalog",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.AlterColumn<string>(
                name: "GoogleMapsPlaceId",
                schema: "Catalog",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: string.Empty,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
}