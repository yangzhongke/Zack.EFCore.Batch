using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Demo.MSSQLServer.Migrations
{
    public partial class OwnedType1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NodaTimeEntities");

            migrationBuilder.AddColumn<string>(
                name: "Remarks_Chinese",
                table: "T_Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks_English",
                table: "T_Articles",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks_Chinese",
                table: "T_Articles");

            migrationBuilder.DropColumn(
                name: "Remarks_English",
                table: "T_Articles");

            migrationBuilder.CreateTable(
                name: "NodaTimeEntities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Instant = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodaTimeEntities", x => x.Id);
                });
        }
    }
}
