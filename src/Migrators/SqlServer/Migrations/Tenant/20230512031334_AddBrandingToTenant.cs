#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Tenant;

public partial class AddBrandingToTenant : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.AddColumn<string>(
                name: "BodyFont",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyFontWoff2Url",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyFontWoffUrl",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeaderFont",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeaderFontWoff2Url",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeaderFontWoffUrl",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoImageUrl",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryHoverColor",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryColor",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryHoverColor",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeritaryColor",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeritaryHoverColor",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                schema: "MultiTenancy",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);
        }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropColumn(
                name: "BodyFont",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "BodyFontWoff2Url",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "BodyFontWoffUrl",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "HeaderFont",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "HeaderFontWoff2Url",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "HeaderFontWoffUrl",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "LogoImageUrl",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "PrimaryColor",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "PrimaryHoverColor",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "SecondaryColor",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "SecondaryHoverColor",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TeritaryColor",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TeritaryHoverColor",
                schema: "MultiTenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                schema: "MultiTenancy",
                table: "Tenants");
        }
}