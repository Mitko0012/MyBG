using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserAddToForum8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_ForeignKey",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_ForeignKey",
                table: "Posts",
                column: "ForeignKey",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_ForeignKey",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_ForeignKey",
                table: "Posts",
                column: "ForeignKey",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
