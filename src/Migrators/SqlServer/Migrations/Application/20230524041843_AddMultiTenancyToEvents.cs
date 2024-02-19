#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Application;

public partial class AddMultiTenancyToEvents : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Catalog",
                table: "Events",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.CreateTable(
                name: "Image",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThumbnailImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubTitle1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubTitle2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Href = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PropertyRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DestinationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_Image", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Image_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalSchema: "Catalog",
                        principalTable: "Destinations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Image_PropertyRooms_PropertyRoomId",
                        column: x => x.PropertyRoomId,
                        principalSchema: "Catalog",
                        principalTable: "PropertyRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Image_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "Catalog",
                        principalTable: "Tours",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Image_DestinationId",
                schema: "Catalog",
                table: "Image",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_PropertyRoomId",
                schema: "Catalog",
                table: "Image",
                column: "PropertyRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_TourId",
                schema: "Catalog",
                table: "Image",
                column: "TourId");
        }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropTable(
                name: "Image",
                schema: "Catalog");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Catalog",
                table: "Events");

        }
}