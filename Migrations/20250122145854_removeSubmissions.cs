using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class removeSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PageModel",
                table: "PageModel");

            migrationBuilder.RenameTable(
                name: "PageModel",
                newName: "Pages");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PageImageArr",
                table: "Pages",
                type: "BLOB",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Pages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pages",
                table: "Pages",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Pages",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Pages");

            migrationBuilder.RenameTable(
                name: "Pages",
                newName: "PageModel");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PageImageArr",
                table: "PageModel",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PageModel",
                table: "PageModel",
                column: "Id");
        }
    }
}
