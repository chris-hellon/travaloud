#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Application;

public partial class PropertyImages : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Catalog",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                schema: "Catalog",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            //migrationBuilder.AlterColumn<string>(
            //    name: "GuestId",
            //    schema: "Catalog",
            //    table: "Bookings",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(Guid),
            //    oldType: "uniqueidentifier",
            //    oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PropertyImages",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThumbnailImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyImages_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalSchema: "Catalog",
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_PropertyId",
                schema: "Catalog",
                table: "Events",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyImages_PropertyId",
                schema: "Catalog",
                table: "PropertyImages",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Properties_PropertyId",
                schema: "Catalog",
                table: "Events",
                column: "PropertyId",
                principalSchema: "Catalog",
                principalTable: "Properties",
                principalColumn: "Id");
        }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Properties_PropertyId",
                schema: "Catalog",
                table: "Events");

            migrationBuilder.DropTable(
                name: "PropertyImages",
                schema: "Catalog");

            migrationBuilder.DropIndex(
                name: "IX_Events_PropertyId",
                schema: "Catalog",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                schema: "Catalog",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: string.Empty,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Catalog",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: string.Empty);

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "GuestId",
            //    schema: "Catalog",
            //    table: "Bookings",
            //    type: "uniqueidentifier",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);
        }
}