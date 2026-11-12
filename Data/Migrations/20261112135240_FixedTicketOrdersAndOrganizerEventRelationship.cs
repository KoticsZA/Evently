using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evently.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixedTicketOrdersAndOrganizerEventRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizerEvents_EventOrganizers_OrganizerId",
                table: "OrganizerEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizerEvents_Events_EventId",
                table: "OrganizerEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizerEvents",
                table: "OrganizerEvents");

            migrationBuilder.AddColumn<int>(
                name: "OrganizerEventId",
                table: "TicketOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "OrganizerEvents",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "EventOrganizers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizerEvents",
                table: "OrganizerEvents",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TicketOrders_OrganizerEventId",
                table: "TicketOrders",
                column: "OrganizerEventId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizerEvents_OrganizerId_EventId",
                table: "OrganizerEvents",
                columns: new[] { "OrganizerId", "EventId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizerEvents_EventOrganizers_OrganizerId",
                table: "OrganizerEvents",
                column: "OrganizerId",
                principalTable: "EventOrganizers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizerEvents_Events_EventId",
                table: "OrganizerEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketOrders_OrganizerEvents_OrganizerEventId",
                table: "TicketOrders",
                column: "OrganizerEventId",
                principalTable: "OrganizerEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizerEvents_EventOrganizers_OrganizerId",
                table: "OrganizerEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizerEvents_Events_EventId",
                table: "OrganizerEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketOrders_OrganizerEvents_OrganizerEventId",
                table: "TicketOrders");

            migrationBuilder.DropIndex(
                name: "IX_TicketOrders_OrganizerEventId",
                table: "TicketOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizerEvents",
                table: "OrganizerEvents");

            migrationBuilder.DropIndex(
                name: "IX_OrganizerEvents_OrganizerId_EventId",
                table: "OrganizerEvents");

            migrationBuilder.DropColumn(
                name: "OrganizerEventId",
                table: "TicketOrders");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrganizerEvents");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "EventOrganizers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizerEvents",
                table: "OrganizerEvents",
                columns: new[] { "OrganizerId", "EventId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizerEvents_EventOrganizers_OrganizerId",
                table: "OrganizerEvents",
                column: "OrganizerId",
                principalTable: "EventOrganizers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizerEvents_Events_EventId",
                table: "OrganizerEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
