using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class transportways : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransportWay",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransportWayType = table.Column<int>(type: "INTEGER", nullable: false),
                    TransportTime = table.Column<string>(type: "TEXT", nullable: false),
                    TransportOrigin = table.Column<string>(type: "TEXT", nullable: false),
                    PageKey = table.Column<int>(type: "INTEGER", nullable: false),
                    PageModelId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportWay", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TransportWay_Pages_PageModelId",
                        column: x => x.PageModelId,
                        principalTable: "Pages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransportWay_PageModelId",
                table: "TransportWay",
                column: "PageModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransportWay");
        }
    }
}
