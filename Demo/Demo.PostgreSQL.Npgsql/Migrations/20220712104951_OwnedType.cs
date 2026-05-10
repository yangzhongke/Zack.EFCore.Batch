using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.PostgreSQL.Npgsql.Migrations
{
    public partial class OwnedType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "PubTime",
                schema: "MySchema1",
                table: "T_Books",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "T_Authors",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Remarks_Chinese",
                table: "T_Articles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks_English",
                table: "T_Articles",
                type: "text",
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

            migrationBuilder.AlterColumn<DateTime>(
                name: "PubTime",
                schema: "MySchema1",
                table: "T_Books",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "T_Authors",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
