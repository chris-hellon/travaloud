#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Tenant;

public partial class AddUrlToTenant2 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);
        }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropColumn(
                name: "Url",
                schema: "MultiTenancy",
                table: "Tenants");
        }
}