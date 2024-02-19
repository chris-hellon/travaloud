#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Application;

public partial class JobVacanciesAndPageModals : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.AlterColumn<string>(
                name: "JobTitle",
                schema: "Catalog",
                table: "JobVacancies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CallToAction",
                schema: "Catalog",
                table: "JobVacancies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Catalog",
                table: "JobVacancies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JobVacancyResponses",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    HowCanWeCollaborate = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    EstimatedDates = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DestinationsVisited = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Instagram = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TikTok = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    YouTube = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Portfolio = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Equipment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    JobVacancyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_JobVacancyResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobVacancyResponses_JobVacancies_JobVacancyId",
                        column: x => x.JobVacancyId,
                        principalSchema: "Catalog",
                        principalTable: "JobVacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageModals",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallToAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_PageModals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageModals_Pages_PageId",
                        column: x => x.PageId,
                        principalSchema: "Catalog",
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobVacancyResponses_JobVacancyId",
                schema: "Catalog",
                table: "JobVacancyResponses",
                column: "JobVacancyId");

            migrationBuilder.CreateIndex(
                name: "IX_PageModals_PageId",
                schema: "Catalog",
                table: "PageModals",
                column: "PageId");
        }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropTable(
                name: "JobVacancyResponses",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "PageModals",
                schema: "Catalog");

            migrationBuilder.DropColumn(
                name: "CallToAction",
                schema: "Catalog",
                table: "JobVacancies");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Catalog",
                table: "JobVacancies");

            migrationBuilder.AlterColumn<string>(
                name: "JobTitle",
                schema: "Catalog",
                table: "JobVacancies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
}