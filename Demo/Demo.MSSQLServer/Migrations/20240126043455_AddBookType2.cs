using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.MSSQLServer.Migrations
{
    public partial class AddBookType2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookType2",
                schema: "MySchema1",
                table: "T_Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookType2",
                schema: "MySchema1",
                table: "T_Books");
        }
    }
}
