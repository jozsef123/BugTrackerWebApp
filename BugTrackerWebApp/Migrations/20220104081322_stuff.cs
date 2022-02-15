using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTrackerWebApp.Migrations
{
    public partial class stuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedDeveloperId",
                table: "Ticket_History");

            migrationBuilder.DropColumn(
                name: "NewValueId",
                table: "Ticket_History");

            migrationBuilder.DropColumn(
                name: "OldValueId",
                table: "Ticket_History");

            migrationBuilder.DropColumn(
                name: "UserThatUpdatedTicketId",
                table: "Ticket_History");

            migrationBuilder.DropColumn(
                name: "AssignedDeveloperId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "SubmitterId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "File");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Comment");

            migrationBuilder.AddColumn<string>(
                name: "AssignedDeveloperUserName",
                table: "Ticket_History",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewValueUserName",
                table: "Ticket_History",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldValueUserName",
                table: "Ticket_History",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TicketUpdaterUserName",
                table: "Ticket_History",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedDeveloperUserName",
                table: "Ticket",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmitterUserName",
                table: "Ticket",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmitterUserName",
                table: "File",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmitterUserName",
                table: "Comment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedDeveloperUserName",
                table: "Ticket_History");

            migrationBuilder.DropColumn(
                name: "NewValueUserName",
                table: "Ticket_History");

            migrationBuilder.DropColumn(
                name: "OldValueUserName",
                table: "Ticket_History");

            migrationBuilder.DropColumn(
                name: "TicketUpdaterUserName",
                table: "Ticket_History");

            migrationBuilder.DropColumn(
                name: "AssignedDeveloperUserName",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "SubmitterUserName",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "SubmitterUserName",
                table: "File");

            migrationBuilder.DropColumn(
                name: "SubmitterUserName",
                table: "Comment");

            migrationBuilder.AddColumn<string>(
                name: "AssignedDeveloperId",
                table: "Ticket_History",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewValueId",
                table: "Ticket_History",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldValueId",
                table: "Ticket_History",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserThatUpdatedTicketId",
                table: "Ticket_History",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedDeveloperId",
                table: "Ticket",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmitterId",
                table: "Ticket",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "File",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Comment",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
