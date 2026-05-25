using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.PostgreSQL.Npgsql.Migrations
{
    public partial class AddBookType2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookType2",
                schema: "MySchema1",
                table: "T_Books",
                type: "integer",
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
