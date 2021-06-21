using Microsoft.EntityFrameworkCore.Migrations;

namespace Demo.MSSQLServer.Migrations
{
    public partial class AddNullableProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Pages",
                schema: "MySchema1",
                table: "T_Books",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pages",
                schema: "MySchema1",
                table: "T_Books");
        }
    }
}
