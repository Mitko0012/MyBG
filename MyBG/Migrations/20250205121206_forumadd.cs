using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class forumadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "Date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommentModelForumQuestion",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "INTEGER", nullable: false),
                    PostedOnForumsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentModelForumQuestion", x => new { x.CommentId, x.PostedOnForumsId });
                    table.ForeignKey(
                        name: "FK_CommentModelForumQuestion_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentModelForumQuestion_Posts_PostedOnForumsId",
                        column: x => x.PostedOnForumsId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForumQuestionPFPModel",
                columns: table => new
                {
                    LikedUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    UpvotedForumsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumQuestionPFPModel", x => new { x.LikedUserId, x.UpvotedForumsId });
                    table.ForeignKey(
                        name: "FK_ForumQuestionPFPModel_PFPs_LikedUserId",
                        column: x => x.LikedUserId,
                        principalTable: "PFPs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForumQuestionPFPModel_Posts_UpvotedForumsId",
                        column: x => x.UpvotedForumsId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentModelForumQuestion_PostedOnForumsId",
                table: "CommentModelForumQuestion",
                column: "PostedOnForumsId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumQuestionPFPModel_UpvotedForumsId",
                table: "ForumQuestionPFPModel",
                column: "UpvotedForumsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentModelForumQuestion");

            migrationBuilder.DropTable(
                name: "ForumQuestionPFPModel");

            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
