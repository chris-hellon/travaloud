#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.SqlServer.Migrations.Application;

public partial class IgnoreInvoiceId2 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropColumn(name: "BookingReference", "Bookings", "Catalog");
            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "Bookings",
                type: "int",
                schema: "Catalog",
                nullable: false).Annotation("SqlServer:Identity", "1, 1");
        }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
            //migrationBuilder.RenameColumn(
            //    name: "BookingReference",
            //    schema: "Catalog",
            //    table: "Bookings",
            //    newName: "InvoiceId");

            //migrationBuilder.AlterColumn<int>(
            //    name: "InvoiceId",
            //    schema: "Catalog",
            //    table: "Bookings",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            //migrationBuilder.DropColumn(name: "BookingReference", "Bookings", "Catalog");
            //migrationBuilder.AddColumn<int>(
            //    name: "InvoiceId",
            //    table: "Bookings",
            //    type: "int",
            //    schema: "Catalog",
            //    nullable: false).Annotation("SqlServer:Identity", "1, 1");
        }
}