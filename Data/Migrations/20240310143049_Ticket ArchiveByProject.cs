using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTracks.Data.Migrations
{
    /// <inheritdoc />
    public partial class TicketArchiveByProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ArchivedByProject",
                table: "Tickets",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivedByProject",
                table: "Tickets");
        }
    }
}
