using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class comment6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_PFPs_PFPId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "PFPId",
                table: "Comments",
                newName: "PFPModelId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_PFPId",
                table: "Comments",
                newName: "IX_Comments_PFPModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_PFPs_PFPModelId",
                table: "Comments",
                column: "PFPModelId",
                principalTable: "PFPs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_PFPs_PFPModelId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "PFPModelId",
                table: "Comments",
                newName: "PFPId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_PFPModelId",
                table: "Comments",
                newName: "IX_Comments_PFPId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_PFPs_PFPId",
                table: "Comments",
                column: "PFPId",
                principalTable: "PFPs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
