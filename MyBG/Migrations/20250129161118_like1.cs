using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class like1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PageLikeId",
                table: "PFPs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PFPModelId",
                table: "Pages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PFPModelId",
                table: "Pages",
                column: "PFPModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_PFPs_PFPModelId",
                table: "Pages",
                column: "PFPModelId",
                principalTable: "PFPs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_PFPs_PFPModelId",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_PFPModelId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "PageLikeId",
                table: "PFPs");

            migrationBuilder.DropColumn(
                name: "PFPModelId",
                table: "Pages");
        }
    }
}
