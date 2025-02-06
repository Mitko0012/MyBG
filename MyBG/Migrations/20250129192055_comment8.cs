using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class comment8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_PFPs_PFPModelId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PFPModelId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "PFPModelId",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "PFPSrc",
                table: "Comments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PFPSrc",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "PFPModelId",
                table: "Comments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PFPModelId",
                table: "Comments",
                column: "PFPModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_PFPs_PFPModelId",
                table: "Comments",
                column: "PFPModelId",
                principalTable: "PFPs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
