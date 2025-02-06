using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class like : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PageModelId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PageModelId",
                table: "AspNetUsers",
                column: "PageModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Pages_PageModelId",
                table: "AspNetUsers",
                column: "PageModelId",
                principalTable: "Pages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Pages_PageModelId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PageModelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PageModelId",
                table: "AspNetUsers");
        }
    }
}
