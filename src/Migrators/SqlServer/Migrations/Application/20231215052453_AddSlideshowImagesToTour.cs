#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Application;

public partial class AddSlideshowImagesToTour : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Tours_TourId",
                schema: "Catalog",
                table: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Image_TourId",
                schema: "Catalog",
                table: "Image");

            migrationBuilder.CreateTable(
                name: "TourImages",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_TourImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourImages_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "Catalog",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourImages_TourId",
                schema: "Catalog",
                table: "TourImages",
                column: "TourId");
        }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropTable(
                name: "TourImages",
                schema: "Catalog");

            migrationBuilder.CreateIndex(
                name: "IX_Image_TourId",
                schema: "Catalog",
                table: "Image",
                column: "TourId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Tours_TourId",
                schema: "Catalog",
                table: "Image",
                column: "TourId",
                principalSchema: "Catalog",
                principalTable: "Tours",
                principalColumn: "Id");
        }
}