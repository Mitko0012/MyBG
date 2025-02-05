using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class skibidi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "CommentModelPFPModel",
                columns: table => new
                {
                    CommentsLikedId = table.Column<int>(type: "INTEGER", nullable: false),
                    LikedUserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentModelPFPModel", x => new { x.CommentsLikedId, x.LikedUserId });
                    table.ForeignKey(
                        name: "FK_CommentModelPFPModel_Comments_CommentsLikedId",
                        column: x => x.CommentsLikedId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentModelPFPModel_PFPs_LikedUserId",
                        column: x => x.LikedUserId,
                        principalTable: "PFPs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentModelPFPModel_LikedUserId",
                table: "CommentModelPFPModel",
                column: "LikedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentModelPFPModel");

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
    }
}
