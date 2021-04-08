using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Demo.Oracle.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Books",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "1, 1"),
                    Title = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PubTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Price = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    AuthorName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Books", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Books");
        }
    }
}
