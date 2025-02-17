using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserAddToForum6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_UserId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Posts");

            migrationBuilder.AlterColumn<string>(
                name: "ForeignKey",
                table: "Posts",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ForeignKey",
                table: "Posts",
                column: "ForeignKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_ForeignKey",
                table: "Posts",
                column: "ForeignKey",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_ForeignKey",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_ForeignKey",
                table: "Posts");

            migrationBuilder.AlterColumn<int>(
                name: "ForeignKey",
                table: "Posts",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Posts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
