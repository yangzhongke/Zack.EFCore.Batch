using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Sqlite.Migrations
{
    public partial class AddBookType2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "MySchema1",
                table: "T_Books",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                schema: "MySchema1",
                table: "T_Books",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "Pages",
                schema: "MySchema1",
                table: "T_Books",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorName",
                schema: "MySchema1",
                table: "T_Books",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "BookType",
                schema: "MySchema1",
                table: "T_Books",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BookType2",
                schema: "MySchema1",
                table: "T_Books",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "PKId",
                table: "T_Articles",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks_Chinese",
                table: "T_Articles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks_English",
                table: "T_Articles",
                type: "TEXT",
                nullable: true);
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

            migrationBuilder.DropColumn(
                name: "Remarks_Chinese",
                table: "T_Articles");

            migrationBuilder.DropColumn(
                name: "Remarks_English",
                table: "T_Articles");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "MySchema1",
                table: "T_Books",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                schema: "MySchema1",
                table: "T_Books",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "Pages",
                schema: "MySchema1",
                table: "T_Books",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorName",
                schema: "MySchema1",
                table: "T_Books",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "PKId",
                table: "T_Articles",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);
        }
    }
}
