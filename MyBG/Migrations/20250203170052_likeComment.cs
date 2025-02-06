using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class likeComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentModelId",
                table: "PFPs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PFPModelId",
                table: "PFPs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PFPs_CommentModelId",
                table: "PFPs",
                column: "CommentModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PFPs_PFPModelId",
                table: "PFPs",
                column: "PFPModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PFPs_Comments_CommentModelId",
                table: "PFPs",
                column: "CommentModelId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PFPs_PFPs_PFPModelId",
                table: "PFPs",
                column: "PFPModelId",
                principalTable: "PFPs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PFPs_Comments_CommentModelId",
                table: "PFPs");

            migrationBuilder.DropForeignKey(
                name: "FK_PFPs_PFPs_PFPModelId",
                table: "PFPs");

            migrationBuilder.DropIndex(
                name: "IX_PFPs_CommentModelId",
                table: "PFPs");

            migrationBuilder.DropIndex(
                name: "IX_PFPs_PFPModelId",
                table: "PFPs");

            migrationBuilder.DropColumn(
                name: "CommentModelId",
                table: "PFPs");

            migrationBuilder.DropColumn(
                name: "PFPModelId",
                table: "PFPs");
        }
    }
}
