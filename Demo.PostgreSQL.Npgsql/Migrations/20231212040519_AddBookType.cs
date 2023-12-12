using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.PostgreSQL.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddBookType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "MySchema1",
                table: "T_Books",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                schema: "MySchema1",
                table: "T_Books",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "Pages",
                schema: "MySchema1",
                table: "T_Books",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorName",
                schema: "MySchema1",
                table: "T_Books",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "BookType",
                schema: "MySchema1",
                table: "T_Books",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookType",
                schema: "MySchema1",
                table: "T_Books");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "MySchema1",
                table: "T_Books",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                schema: "MySchema1",
                table: "T_Books",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "Pages",
                schema: "MySchema1",
                table: "T_Books",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorName",
                schema: "MySchema1",
                table: "T_Books",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
