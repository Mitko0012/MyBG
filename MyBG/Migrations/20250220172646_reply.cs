using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class reply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentModelId",
                table: "Comments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentModelId",
                table: "Comments",
                column: "CommentModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_CommentModelId",
                table: "Comments",
                column: "CommentModelId",
                principalTable: "Comments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_CommentModelId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentModelId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CommentModelId",
                table: "Comments");
        }
    }
}
