using Microsoft.EntityFrameworkCore.Migrations;

namespace Demo.PostgreSQL.Npgsql.Migrations
{
    public partial class AddNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Pages",
                schema: "MySchema1",
                table: "T_Books",
                type: "integer",
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
