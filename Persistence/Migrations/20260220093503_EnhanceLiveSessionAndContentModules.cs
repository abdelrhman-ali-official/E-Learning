using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceLiveSessionAndContentModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "YouTubeLiveVideoId",
                table: "LiveSessions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "InstructorId",
                table: "LiveSessions",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MeetingLink",
                table: "LiveSessions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LiveSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ExternalVideoUrl",
                table: "Contents",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessions_InstructorId",
                table: "LiveSessions",
                column: "InstructorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LiveSessions_InstructorId",
                table: "LiveSessions");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "LiveSessions");

            migrationBuilder.DropColumn(
                name: "MeetingLink",
                table: "LiveSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LiveSessions");

            migrationBuilder.DropColumn(
                name: "ExternalVideoUrl",
                table: "Contents");

            migrationBuilder.AlterColumn<string>(
                name: "YouTubeLiveVideoId",
                table: "LiveSessions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
