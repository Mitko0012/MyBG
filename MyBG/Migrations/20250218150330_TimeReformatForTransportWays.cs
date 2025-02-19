using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class TimeReformatForTransportWays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransportTime",
                table: "TransportWays");

            migrationBuilder.AddColumn<int>(
                name: "TransportTimeHours",
                table: "TransportWays",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransportTimeMinutes",
                table: "TransportWays",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransportTimeHours",
                table: "TransportWays");

            migrationBuilder.DropColumn(
                name: "TransportTimeMinutes",
                table: "TransportWays");

            migrationBuilder.AddColumn<string>(
                name: "TransportTime",
                table: "TransportWays",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
