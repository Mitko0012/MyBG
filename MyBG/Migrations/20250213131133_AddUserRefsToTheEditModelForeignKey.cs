using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRefsToTheEditModelForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Edits_PFPs_UserCreatedId",
                table: "Edits");

            migrationBuilder.RenameColumn(
                name: "UserCreatedId",
                table: "Edits",
                newName: "PFPKey");

            migrationBuilder.RenameIndex(
                name: "IX_Edits_UserCreatedId",
                table: "Edits",
                newName: "IX_Edits_PFPKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Edits_PFPs_PFPKey",
                table: "Edits",
                column: "PFPKey",
                principalTable: "PFPs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Edits_PFPs_PFPKey",
                table: "Edits");

            migrationBuilder.RenameColumn(
                name: "PFPKey",
                table: "Edits",
                newName: "UserCreatedId");

            migrationBuilder.RenameIndex(
                name: "IX_Edits_PFPKey",
                table: "Edits",
                newName: "IX_Edits_UserCreatedId");

            migrationBuilder.AddForeignKey(
                name: "FK_Edits_PFPs_UserCreatedId",
                table: "Edits",
                column: "UserCreatedId",
                principalTable: "PFPs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
