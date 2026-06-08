using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterCRM.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddChatAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "users",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "positions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentMime",
                table: "chat_messages",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentName",
                table: "chat_messages",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentObjectName",
                table: "chat_messages",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AttachmentSize",
                table: "chat_messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageType",
                table: "chat_messages",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "text");

            migrationBuilder.CreateIndex(
                name: "IX_positions_DepartmentId",
                table: "positions",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_positions_departments_DepartmentId",
                table: "positions",
                column: "DepartmentId",
                principalTable: "departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_positions_departments_DepartmentId",
                table: "positions");

            migrationBuilder.DropIndex(
                name: "IX_positions_DepartmentId",
                table: "positions");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "positions");

            migrationBuilder.DropColumn(
                name: "AttachmentMime",
                table: "chat_messages");

            migrationBuilder.DropColumn(
                name: "AttachmentName",
                table: "chat_messages");

            migrationBuilder.DropColumn(
                name: "AttachmentObjectName",
                table: "chat_messages");

            migrationBuilder.DropColumn(
                name: "AttachmentSize",
                table: "chat_messages");

            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "chat_messages");

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
