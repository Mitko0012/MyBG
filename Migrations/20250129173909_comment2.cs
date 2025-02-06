using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class comment2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PFPId",
                table: "Comments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PFPId",
                table: "Comments",
                column: "PFPId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_PFPs_PFPId",
                table: "Comments",
                column: "PFPId",
                principalTable: "PFPs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_PFPs_PFPId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PFPId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "PFPId",
                table: "Comments");
        }
    }
}
