using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBG.Migrations
{
    /// <inheritdoc />
    public partial class NationalityAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Nationality",
                table: "PFPs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "PFPs");
        }
    }
}
