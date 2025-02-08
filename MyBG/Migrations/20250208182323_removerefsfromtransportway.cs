using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class removerefsfromtransportway : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransportWays_Pages_PageKey",
                table: "TransportWays");

            migrationBuilder.DropIndex(
                name: "IX_TransportWays_PageKey",
                table: "TransportWays");

            migrationBuilder.DropColumn(
                name: "PageKey",
                table: "TransportWays");

            migrationBuilder.AddColumn<int>(
                name: "PageModelId",
                table: "TransportWays",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransportWays_PageModelId",
                table: "TransportWays",
                column: "PageModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransportWays_Pages_PageModelId",
                table: "TransportWays",
                column: "PageModelId",
                principalTable: "Pages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransportWays_Pages_PageModelId",
                table: "TransportWays");

            migrationBuilder.DropIndex(
                name: "IX_TransportWays_PageModelId",
                table: "TransportWays");

            migrationBuilder.DropColumn(
                name: "PageModelId",
                table: "TransportWays");

            migrationBuilder.AddColumn<int>(
                name: "PageKey",
                table: "TransportWays",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TransportWays_PageKey",
                table: "TransportWays",
                column: "PageKey");

            migrationBuilder.AddForeignKey(
                name: "FK_TransportWays_Pages_PageKey",
                table: "TransportWays",
                column: "PageKey",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
