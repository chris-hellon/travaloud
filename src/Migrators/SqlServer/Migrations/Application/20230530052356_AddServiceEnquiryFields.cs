#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Application;

public partial class AddServiceEnquiryFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropColumn(
                name: "Field",
                schema: "Catalog",
                table: "ServiceEnquiries");

            migrationBuilder.DropColumn(
                name: "Value",
                schema: "Catalog",
                table: "ServiceEnquiries");

            migrationBuilder.CreateTable(
                name: "ServiceEnquiryFields",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceEnquiryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Field = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_ServiceEnquiryFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceEnquiryFields_ServiceEnquiries_ServiceEnquiryId",
                        column: x => x.ServiceEnquiryId,
                        principalSchema: "Catalog",
                        principalTable: "ServiceEnquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEnquiryFields_ServiceEnquiryId",
                schema: "Catalog",
                table: "ServiceEnquiryFields",
                column: "ServiceEnquiryId");
        }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropTable(
                name: "ServiceEnquiryFields",
                schema: "Catalog");

            migrationBuilder.AddColumn<string>(
                name: "Field",
                schema: "Catalog",
                table: "ServiceEnquiries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                schema: "Catalog",
                table: "ServiceEnquiries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
}