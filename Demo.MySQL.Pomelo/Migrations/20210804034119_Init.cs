using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Demo.MySQL.Pomelo.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_Books",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Title = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    PubTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Price = table.Column<double>(type: "double", nullable: false),
                    AuthorName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Pages = table.Column<int>(type: "int", nullable: true),
                    Available = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "创建时间"),
                    create_user = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "创建者")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    update_user = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "更新者")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    row_rersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true, comment: "行版本号"),
                    is_soft_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否软删除")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Books", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_T_Books_id",
                table: "T_Books",
                column: "id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Books");
        }
    }
}
