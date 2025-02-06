using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class fixlikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Pages_PageModelId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Pages_PFPs_PFPModelId",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_PFPModelId",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PageModelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PFPModelId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "PageModelId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "PFPModelPageModel",
                columns: table => new
                {
                    PagesLikedId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersLikedId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PFPModelPageModel", x => new { x.PagesLikedId, x.UsersLikedId });
                    table.ForeignKey(
                        name: "FK_PFPModelPageModel_PFPs_UsersLikedId",
                        column: x => x.UsersLikedId,
                        principalTable: "PFPs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PFPModelPageModel_Pages_PagesLikedId",
                        column: x => x.PagesLikedId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PFPModelPageModel_UsersLikedId",
                table: "PFPModelPageModel",
                column: "UsersLikedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PFPModelPageModel");

            migrationBuilder.AddColumn<int>(
                name: "PFPModelId",
                table: "Pages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageModelId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PFPModelId",
                table: "Pages",
                column: "PFPModelId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_PFPs_PFPModelId",
                table: "Pages",
                column: "PFPModelId",
                principalTable: "PFPs",
                principalColumn: "Id");
        }
    }
}
