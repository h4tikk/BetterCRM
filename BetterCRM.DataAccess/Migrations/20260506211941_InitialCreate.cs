using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterCRM.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MainDirectorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "positions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    HourlyRate = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    DailyNormHours = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_positions_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_positions_organizations_OrganizationId1",
                        column: x => x.OrganizationId1,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    FullName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    HireDate = table.Column<DateTime>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DepartmentEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    PositionEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_departments_DepartmentEntityId",
                        column: x => x.DepartmentEntityId,
                        principalTable: "departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_users_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_users_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_organizations_OrganizationId1",
                        column: x => x.OrganizationId1,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_users_positions_PositionEntityId",
                        column: x => x.PositionEntityId,
                        principalTable: "positions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_users_positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payroll_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "date", nullable: false),
                    ScheduledHours = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    ActualHours = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    AttendancePenaltyHours = table.Column<decimal>(type: "numeric(6,2)", nullable: false, defaultValue: 0m),
                    TicketPenaltyHours = table.Column<decimal>(type: "numeric(6,2)", nullable: false, defaultValue: 0m),
                    TotalPenaltyHours = table.Column<decimal>(type: "numeric(6,2)", nullable: false, defaultValue: 0m),
                    FinalBillableHours = table.Column<decimal>(type: "numeric(6,2)", nullable: false, defaultValue: 0m),
                    HourlyRate = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    CalculatedSalary = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payroll_records_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_payroll_records_organizations_OrganizationId1",
                        column: x => x.OrganizationId1,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_payroll_records_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LatenessPenaltyHours = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    EarlyLeavePenaltyHours = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Status = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_shifts_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shifts_organizations_OrganizationId1",
                        column: x => x.OrganizationId1,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_shifts_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Priority = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssigneeId = table.Column<Guid>(type: "uuid", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    SLATargetHours = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    IsSLABreached = table.Column<bool>(type: "boolean", nullable: false),
                    OverduePenaltyHours = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tickets_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tickets_organizations_OrganizationId1",
                        column: x => x.OrganizationId1,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tickets_users_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_tickets_users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    Comment = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_work_sessions_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_sessions_organizations_OrganizationId1",
                        column: x => x.OrganizationId1,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_work_sessions_shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_work_sessions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ticket_participants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    Role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    UserEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket_participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ticket_participants_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ticket_participants_organizations_OrganizationId1",
                        column: x => x.OrganizationId1,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ticket_participants_tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ticket_participants_users_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ticket_participants_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "time_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    DurationHours = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_time_logs_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_time_logs_organizations_OrganizationId1",
                        column: x => x.OrganizationId1,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_time_logs_tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_time_logs_work_sessions_WorkSessionId",
                        column: x => x.WorkSessionId,
                        principalTable: "work_sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_departments_OrganizationId_Name",
                table: "departments",
                columns: new[] { "OrganizationId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_departments_OrganizationId1",
                table: "departments",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_organizations_MainDirectorId",
                table: "organizations",
                column: "MainDirectorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_organizations_Slug",
                table: "organizations",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payroll_records_OrganizationId_UserId_PeriodStart",
                table: "payroll_records",
                columns: new[] { "OrganizationId", "UserId", "PeriodStart" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payroll_records_OrganizationId1",
                table: "payroll_records",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_records_UserId",
                table: "payroll_records",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_positions_OrganizationId",
                table: "positions",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_positions_OrganizationId1",
                table: "positions",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_shifts_OrganizationId_UserId_Date",
                table: "shifts",
                columns: new[] { "OrganizationId", "UserId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shifts_OrganizationId1",
                table: "shifts",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_shifts_UserId",
                table: "shifts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_participants_OrganizationId_TicketId_UserId",
                table: "ticket_participants",
                columns: new[] { "OrganizationId", "TicketId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ticket_participants_OrganizationId1",
                table: "ticket_participants",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_participants_TicketId",
                table: "ticket_participants",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_participants_UserEntityId",
                table: "ticket_participants",
                column: "UserEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_participants_UserId",
                table: "ticket_participants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_AssigneeId",
                table: "tickets",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_CreatorId",
                table: "tickets",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_OrganizationId_DepartmentId",
                table: "tickets",
                columns: new[] { "OrganizationId", "DepartmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_tickets_OrganizationId_Status_CreatedAt",
                table: "tickets",
                columns: new[] { "OrganizationId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_tickets_OrganizationId1",
                table: "tickets",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_time_logs_OrganizationId_TicketId",
                table: "time_logs",
                columns: new[] { "OrganizationId", "TicketId" });

            migrationBuilder.CreateIndex(
                name: "IX_time_logs_OrganizationId1",
                table: "time_logs",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_time_logs_TicketId",
                table: "time_logs",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_time_logs_WorkSessionId",
                table: "time_logs",
                column: "WorkSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_users_DepartmentEntityId",
                table: "users",
                column: "DepartmentEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_users_DepartmentId",
                table: "users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_users_OrganizationId_Email",
                table: "users",
                columns: new[] { "OrganizationId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_OrganizationId1",
                table: "users",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_users_PositionEntityId",
                table: "users",
                column: "PositionEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_users_PositionId",
                table: "users",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_work_sessions_OrganizationId_UserId_StartedAt",
                table: "work_sessions",
                columns: new[] { "OrganizationId", "UserId", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_work_sessions_OrganizationId1",
                table: "work_sessions",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_work_sessions_ShiftId",
                table: "work_sessions",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_work_sessions_UserId",
                table: "work_sessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_departments_organizations_OrganizationId",
                table: "departments",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_departments_organizations_OrganizationId1",
                table: "departments",
                column: "OrganizationId1",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_organizations_users_MainDirectorId",
                table: "organizations",
                column: "MainDirectorId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departments_organizations_OrganizationId",
                table: "departments");

            migrationBuilder.DropForeignKey(
                name: "FK_departments_organizations_OrganizationId1",
                table: "departments");

            migrationBuilder.DropForeignKey(
                name: "FK_positions_organizations_OrganizationId",
                table: "positions");

            migrationBuilder.DropForeignKey(
                name: "FK_positions_organizations_OrganizationId1",
                table: "positions");

            migrationBuilder.DropForeignKey(
                name: "FK_users_organizations_OrganizationId",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_organizations_OrganizationId1",
                table: "users");

            migrationBuilder.DropTable(
                name: "payroll_records");

            migrationBuilder.DropTable(
                name: "ticket_participants");

            migrationBuilder.DropTable(
                name: "time_logs");

            migrationBuilder.DropTable(
                name: "tickets");

            migrationBuilder.DropTable(
                name: "work_sessions");

            migrationBuilder.DropTable(
                name: "shifts");

            migrationBuilder.DropTable(
                name: "organizations");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropTable(
                name: "positions");
        }
    }
}
