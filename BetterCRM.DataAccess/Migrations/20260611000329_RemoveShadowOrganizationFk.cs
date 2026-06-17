using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterCRM.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShadowOrganizationFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departments_organizations_OrganizationId1",
                table: "departments");

            migrationBuilder.DropForeignKey(
                name: "FK_payroll_records_organizations_OrganizationId1",
                table: "payroll_records");

            migrationBuilder.DropForeignKey(
                name: "FK_positions_organizations_OrganizationId1",
                table: "positions");

            migrationBuilder.DropForeignKey(
                name: "FK_shift_breaks_organizations_OrganizationId1",
                table: "shift_breaks");

            migrationBuilder.DropForeignKey(
                name: "FK_shifts_organizations_OrganizationId1",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_ticket_participants_organizations_OrganizationId1",
                table: "ticket_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_tickets_organizations_OrganizationId1",
                table: "tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_time_logs_organizations_OrganizationId1",
                table: "time_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_users_organizations_OrganizationId1",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_work_sessions_organizations_OrganizationId1",
                table: "work_sessions");

            migrationBuilder.DropIndex(
                name: "IX_work_sessions_OrganizationId1",
                table: "work_sessions");

            migrationBuilder.DropIndex(
                name: "IX_users_OrganizationId1",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_time_logs_OrganizationId1",
                table: "time_logs");

            migrationBuilder.DropIndex(
                name: "IX_tickets_OrganizationId1",
                table: "tickets");

            migrationBuilder.DropIndex(
                name: "IX_ticket_participants_OrganizationId1",
                table: "ticket_participants");

            migrationBuilder.DropIndex(
                name: "IX_shifts_OrganizationId1",
                table: "shifts");

            migrationBuilder.DropIndex(
                name: "IX_shift_breaks_OrganizationId1",
                table: "shift_breaks");

            migrationBuilder.DropIndex(
                name: "IX_positions_OrganizationId1",
                table: "positions");

            migrationBuilder.DropIndex(
                name: "IX_payroll_records_OrganizationId1",
                table: "payroll_records");

            migrationBuilder.DropIndex(
                name: "IX_departments_OrganizationId1",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "OrganizationId1",
                table: "work_sessions");

            migrationBuilder.DropColumn(
                name: "OrganizationId1",
                table: "users");

            migrationBuilder.DropColumn(
                name: "OrganizationId1",
                table: "time_logs");

            migrationBuilder.DropColumn(
                name: "OrganizationId1",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "OrganizationId1",
                table: "ticket_participants");

            migrationBuilder.DropColumn(
                name: "OrganizationId1",
                table: "shifts");

            migrationBuilder.DropColumn(
                name: "OrganizationId1",
                table: "shift_breaks");

            migrationBuilder.DropColumn(
                name: "OrganizationId1",
                table: "positions");

            migrationBuilder.DropColumn(
                name: "OrganizationId1",
                table: "payroll_records");

            migrationBuilder.DropColumn(
                name: "OrganizationId1",
                table: "departments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId1",
                table: "work_sessions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId1",
                table: "users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId1",
                table: "time_logs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId1",
                table: "tickets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId1",
                table: "ticket_participants",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId1",
                table: "shifts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId1",
                table: "shift_breaks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId1",
                table: "positions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId1",
                table: "payroll_records",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId1",
                table: "departments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_work_sessions_OrganizationId1",
                table: "work_sessions",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_users_OrganizationId1",
                table: "users",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_time_logs_OrganizationId1",
                table: "time_logs",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_OrganizationId1",
                table: "tickets",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_participants_OrganizationId1",
                table: "ticket_participants",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_shifts_OrganizationId1",
                table: "shifts",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_shift_breaks_OrganizationId1",
                table: "shift_breaks",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_positions_OrganizationId1",
                table: "positions",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_records_OrganizationId1",
                table: "payroll_records",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_departments_OrganizationId1",
                table: "departments",
                column: "OrganizationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_departments_organizations_OrganizationId1",
                table: "departments",
                column: "OrganizationId1",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_payroll_records_organizations_OrganizationId1",
                table: "payroll_records",
                column: "OrganizationId1",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_positions_organizations_OrganizationId1",
                table: "positions",
                column: "OrganizationId1",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_shift_breaks_organizations_OrganizationId1",
                table: "shift_breaks",
                column: "OrganizationId1",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_shifts_organizations_OrganizationId1",
                table: "shifts",
                column: "OrganizationId1",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ticket_participants_organizations_OrganizationId1",
                table: "ticket_participants",
                column: "OrganizationId1",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_organizations_OrganizationId1",
                table: "tickets",
                column: "OrganizationId1",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_time_logs_organizations_OrganizationId1",
                table: "time_logs",
                column: "OrganizationId1",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_organizations_OrganizationId1",
                table: "users",
                column: "OrganizationId1",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_work_sessions_organizations_OrganizationId1",
                table: "work_sessions",
                column: "OrganizationId1",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
