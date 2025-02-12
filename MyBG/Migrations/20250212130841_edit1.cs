using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class edit1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Edits_Pages_PageToEditId",
                table: "Edits");

            migrationBuilder.DropIndex(
                name: "IX_Edits_PageToEditId",
                table: "Edits");

            migrationBuilder.DropColumn(
                name: "PageToEditId",
                table: "Edits");

            migrationBuilder.CreateIndex(
                name: "IX_Edits_PageModelKey",
                table: "Edits",
                column: "PageModelKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Edits_Pages_PageModelKey",
                table: "Edits",
                column: "PageModelKey",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Edits_Pages_PageModelKey",
                table: "Edits");

            migrationBuilder.DropIndex(
                name: "IX_Edits_PageModelKey",
                table: "Edits");

            migrationBuilder.AddColumn<int>(
                name: "PageToEditId",
                table: "Edits",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Edits_PageToEditId",
                table: "Edits",
                column: "PageToEditId");

            migrationBuilder.AddForeignKey(
                name: "FK_Edits_Pages_PageToEditId",
                table: "Edits",
                column: "PageToEditId",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
