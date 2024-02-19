#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Application
{
    public partial class PageModalLookups2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageModalLookup_PageModals_PageModalId",
                schema: "Catalog",
                table: "PageModalLookup");

            migrationBuilder.DropForeignKey(
                name: "FK_PageModalLookup_Pages_PageId",
                schema: "Catalog",
                table: "PageModalLookup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PageModalLookup",
                schema: "Catalog",
                table: "PageModalLookup");

            migrationBuilder.RenameTable(
                name: "PageModalLookup",
                schema: "Catalog",
                newName: "PageModalLookups",
                newSchema: "Catalog");

            migrationBuilder.RenameIndex(
                name: "IX_PageModalLookup_PageModalId",
                schema: "Catalog",
                table: "PageModalLookups",
                newName: "IX_PageModalLookups_PageModalId");

            migrationBuilder.RenameIndex(
                name: "IX_PageModalLookup_PageId",
                schema: "Catalog",
                table: "PageModalLookups",
                newName: "IX_PageModalLookups_PageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PageModalLookups",
                schema: "Catalog",
                table: "PageModalLookups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PageModalLookups_PageModals_PageModalId",
                schema: "Catalog",
                table: "PageModalLookups",
                column: "PageModalId",
                principalSchema: "Catalog",
                principalTable: "PageModals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PageModalLookups_Pages_PageId",
                schema: "Catalog",
                table: "PageModalLookups",
                column: "PageId",
                principalSchema: "Catalog",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageModalLookups_PageModals_PageModalId",
                schema: "Catalog",
                table: "PageModalLookups");

            migrationBuilder.DropForeignKey(
                name: "FK_PageModalLookups_Pages_PageId",
                schema: "Catalog",
                table: "PageModalLookups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PageModalLookups",
                schema: "Catalog",
                table: "PageModalLookups");

            migrationBuilder.RenameTable(
                name: "PageModalLookups",
                schema: "Catalog",
                newName: "PageModalLookup",
                newSchema: "Catalog");

            migrationBuilder.RenameIndex(
                name: "IX_PageModalLookups_PageModalId",
                schema: "Catalog",
                table: "PageModalLookup",
                newName: "IX_PageModalLookup_PageModalId");

            migrationBuilder.RenameIndex(
                name: "IX_PageModalLookups_PageId",
                schema: "Catalog",
                table: "PageModalLookup",
                newName: "IX_PageModalLookup_PageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PageModalLookup",
                schema: "Catalog",
                table: "PageModalLookup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PageModalLookup_PageModals_PageModalId",
                schema: "Catalog",
                table: "PageModalLookup",
                column: "PageModalId",
                principalSchema: "Catalog",
                principalTable: "PageModals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PageModalLookup_Pages_PageId",
                schema: "Catalog",
                table: "PageModalLookup",
                column: "PageId",
                principalSchema: "Catalog",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
