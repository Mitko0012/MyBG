using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class transportwayrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransportWay_Pages_PageModelId",
                table: "TransportWay");

            migrationBuilder.DropIndex(
                name: "IX_TransportWay_PageModelId",
                table: "TransportWay");

            migrationBuilder.DropColumn(
                name: "PageModelId",
                table: "TransportWay");

            migrationBuilder.CreateIndex(
                name: "IX_TransportWay_PageKey",
                table: "TransportWay",
                column: "PageKey");

            migrationBuilder.AddForeignKey(
                name: "FK_TransportWay_Pages_PageKey",
                table: "TransportWay",
                column: "PageKey",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransportWay_Pages_PageKey",
                table: "TransportWay");

            migrationBuilder.DropIndex(
                name: "IX_TransportWay_PageKey",
                table: "TransportWay");

            migrationBuilder.AddColumn<int>(
                name: "PageModelId",
                table: "TransportWay",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransportWay_PageModelId",
                table: "TransportWay",
                column: "PageModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransportWay_Pages_PageModelId",
                table: "TransportWay",
                column: "PageModelId",
                principalTable: "Pages",
                principalColumn: "Id");
        }
    }
}
