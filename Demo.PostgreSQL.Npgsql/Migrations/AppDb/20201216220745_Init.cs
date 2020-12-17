using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Demo.PostgreSQL.Npgsql.Migrations.AppDb
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    uid = table.Column<string>(type: "text", nullable: false),
                    ip = table.Column<ValueTuple<IPAddress, int>>(type: "cidr", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.uid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
