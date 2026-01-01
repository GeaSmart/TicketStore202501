using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStore.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class newproperyconcert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExtendedDescription",
                schema: "Musicales",
                table: "Concert",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtendedDescription",
                schema: "Musicales",
                table: "Concert");
        }
    }
}
