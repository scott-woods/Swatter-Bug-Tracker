using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTracker.Migrations
{
    public partial class updated_tickets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_AssignedUserId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_SubmitterId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "SubmitterId",
                table: "Tickets",
                newName: "CreatorId");

            migrationBuilder.RenameColumn(
                name: "AssignedUserId",
                table: "Tickets",
                newName: "AssignedDeveloperId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_SubmitterId",
                table: "Tickets",
                newName: "IX_Tickets_CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_AssignedUserId",
                table: "Tickets",
                newName: "IX_Tickets_AssignedDeveloperId");

            migrationBuilder.AddColumn<int>(
                name: "TicketPriority",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TicketStatus",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TicketType",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_AssignedDeveloperId",
                table: "Tickets",
                column: "AssignedDeveloperId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_CreatorId",
                table: "Tickets",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_AssignedDeveloperId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_CreatorId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TicketPriority",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TicketStatus",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TicketType",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Tickets",
                newName: "SubmitterId");

            migrationBuilder.RenameColumn(
                name: "AssignedDeveloperId",
                table: "Tickets",
                newName: "AssignedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_CreatorId",
                table: "Tickets",
                newName: "IX_Tickets_SubmitterId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_AssignedDeveloperId",
                table: "Tickets",
                newName: "IX_Tickets_AssignedUserId");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_AssignedUserId",
                table: "Tickets",
                column: "AssignedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_SubmitterId",
                table: "Tickets",
                column: "SubmitterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
