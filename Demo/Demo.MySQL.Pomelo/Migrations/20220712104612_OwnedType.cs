using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.MySQL.Pomelo_NET6.Migrations
{
    public partial class OwnedType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "PubTime",
                schema: "MySchema1",
                table: "T_Books",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "T_Authors",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Remarks_Chinese",
                table: "T_Articles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Remarks_English",
                table: "T_Articles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
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
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "T_Authors",
                keyColumn: "Tags",
                keyValue: null,
                column: "Tags",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "T_Authors",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
