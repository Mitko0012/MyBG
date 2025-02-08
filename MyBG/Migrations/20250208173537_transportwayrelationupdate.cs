using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class transportwayrelationupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransportWay_Pages_PageKey",
                table: "TransportWay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransportWay",
                table: "TransportWay");

            migrationBuilder.RenameTable(
                name: "TransportWay",
                newName: "TransportWays");

            migrationBuilder.RenameIndex(
                name: "IX_TransportWay_PageKey",
                table: "TransportWays",
                newName: "IX_TransportWays_PageKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransportWays",
                table: "TransportWays",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TransportWays_Pages_PageKey",
                table: "TransportWays",
                column: "PageKey",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransportWays_Pages_PageKey",
                table: "TransportWays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransportWays",
                table: "TransportWays");

            migrationBuilder.RenameTable(
                name: "TransportWays",
                newName: "TransportWay");

            migrationBuilder.RenameIndex(
                name: "IX_TransportWays_PageKey",
                table: "TransportWay",
                newName: "IX_TransportWay_PageKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransportWay",
                table: "TransportWay",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TransportWay_Pages_PageKey",
                table: "TransportWay",
                column: "PageKey",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
