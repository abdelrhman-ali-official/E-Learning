using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCourseIdFromIntToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseVideos_Products_CourseId",
                table: "CourseVideos");

            migrationBuilder.DropForeignKey(
                name: "FK_LiveSessions_Products_CourseId",
                table: "LiveSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoAccessLogs_Products_CourseId",
                table: "VideoAccessLogs");

            // SQL Server cannot ALTER int to uniqueidentifier directly.
            // Drop the old int columns and recreate as uniqueidentifier.
            // Any existing data in these columns will be lost.

            // VideoAccessLogs
            migrationBuilder.DropIndex(
                name: "IX_VideoAccessLogs_CourseId",
                table: "VideoAccessLogs");
            migrationBuilder.DropColumn(name: "CourseId", table: "VideoAccessLogs");
            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "VideoAccessLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty);
            migrationBuilder.CreateIndex(
                name: "IX_VideoAccessLogs_CourseId",
                table: "VideoAccessLogs",
                column: "CourseId");

            // LiveSessions
            migrationBuilder.DropIndex(
                name: "IX_LiveSessions_CourseId",
                table: "LiveSessions");
            migrationBuilder.DropColumn(name: "CourseId", table: "LiveSessions");
            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "LiveSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty);
            migrationBuilder.CreateIndex(
                name: "IX_LiveSessions_CourseId",
                table: "LiveSessions",
                column: "CourseId");

            // CourseVideos
            migrationBuilder.DropIndex(
                name: "IX_CourseVideos_CourseId",
                table: "CourseVideos");
            migrationBuilder.DropColumn(name: "CourseId", table: "CourseVideos");
            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "CourseVideos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty);
            migrationBuilder.CreateIndex(
                name: "IX_CourseVideos_CourseId",
                table: "CourseVideos",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseVideos_Courses_CourseId",
                table: "CourseVideos",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LiveSessions_Courses_CourseId",
                table: "LiveSessions",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoAccessLogs_Courses_CourseId",
                table: "VideoAccessLogs",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseVideos_Courses_CourseId",
                table: "CourseVideos");

            migrationBuilder.DropForeignKey(
                name: "FK_LiveSessions_Courses_CourseId",
                table: "LiveSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoAccessLogs_Courses_CourseId",
                table: "VideoAccessLogs");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "VideoAccessLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "LiveSessions",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "CourseVideos",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseVideos_Products_CourseId",
                table: "CourseVideos",
                column: "CourseId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LiveSessions_Products_CourseId",
                table: "LiveSessions",
                column: "CourseId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoAccessLogs_Products_CourseId",
                table: "VideoAccessLogs",
                column: "CourseId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
