using Microsoft.EntityFrameworkCore.Migrations;

namespace Demo.MSSQLServer.Migrations
{
    public partial class SubOwnedType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Price",
                schema: "MySchema1",
                table: "T_Books",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "Remarks_Second_Name",
                table: "T_Articles",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks_Second_Name",
                table: "T_Articles");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                schema: "MySchema1",
                table: "T_Books",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
