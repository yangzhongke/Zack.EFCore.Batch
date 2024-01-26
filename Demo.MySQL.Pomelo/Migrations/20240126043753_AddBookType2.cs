using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.MySQL.Pomelo_NET6.Migrations
{
    public partial class AddBookType2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "MySchema1",
                table: "T_Books",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                schema: "MySchema1",
                table: "T_Books",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<int>(
                name: "Pages",
                schema: "MySchema1",
                table: "T_Books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorName",
                schema: "MySchema1",
                table: "T_Books",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BookType",
                schema: "MySchema1",
                table: "T_Books",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

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
                name: "BookType",
                schema: "MySchema1",
                table: "T_Books");

            migrationBuilder.DropColumn(
                name: "BookType2",
                schema: "MySchema1",
                table: "T_Books");

            migrationBuilder.UpdateData(
                schema: "MySchema1",
                table: "T_Books",
                keyColumn: "Title",
                keyValue: null,
                column: "Title",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "MySchema1",
                table: "T_Books",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                schema: "MySchema1",
                table: "T_Books",
                type: "double",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "Pages",
                schema: "MySchema1",
                table: "T_Books",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                schema: "MySchema1",
                table: "T_Books",
                keyColumn: "AuthorName",
                keyValue: null,
                column: "AuthorName",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorName",
                schema: "MySchema1",
                table: "T_Books",
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
