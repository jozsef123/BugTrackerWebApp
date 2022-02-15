using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTrackerWebApp.Migrations
{
    public partial class changedusernametoidreference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedDeveloperUserName",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "SubmitterUserName",
                table: "Ticket");

            migrationBuilder.AddColumn<string>(
                name: "AssignedDeveloperId",
                table: "Ticket",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmitterId",
                table: "Ticket",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedDeveloperId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "SubmitterId",
                table: "Ticket");

            migrationBuilder.AddColumn<string>(
                name: "AssignedDeveloperUserName",
                table: "Ticket",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmitterUserName",
                table: "Ticket",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
