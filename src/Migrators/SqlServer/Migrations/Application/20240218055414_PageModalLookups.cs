#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Application
{
    public partial class PageModalLookups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageModals_Pages_PageId",
                schema: "Catalog",
                table: "PageModals");

            migrationBuilder.DropIndex(
                name: "IX_PageModals_PageId",
                schema: "Catalog",
                table: "PageModals");

            migrationBuilder.DropColumn(
                name: "PageId",
                schema: "Catalog",
                table: "PageModals");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "Catalog",
                table: "PageModals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                schema: "Catalog",
                table: "PageModals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PageModalLookup",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PageModalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_PageModalLookup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageModalLookup_PageModals_PageModalId",
                        column: x => x.PageModalId,
                        principalSchema: "Catalog",
                        principalTable: "PageModals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageModalLookup_Pages_PageId",
                        column: x => x.PageId,
                        principalSchema: "Catalog",
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageModalLookup_PageId",
                schema: "Catalog",
                table: "PageModalLookup",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_PageModalLookup_PageModalId",
                schema: "Catalog",
                table: "PageModalLookup",
                column: "PageModalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageModalLookup",
                schema: "Catalog");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "Catalog",
                table: "PageModals");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "Catalog",
                table: "PageModals");

            migrationBuilder.AddColumn<Guid>(
                name: "PageId",
                schema: "Catalog",
                table: "PageModals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PageModals_PageId",
                schema: "Catalog",
                table: "PageModals",
                column: "PageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageModals_Pages_PageId",
                schema: "Catalog",
                table: "PageModals",
                column: "PageId",
                principalSchema: "Catalog",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
