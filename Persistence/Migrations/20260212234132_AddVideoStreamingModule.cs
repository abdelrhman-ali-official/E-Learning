using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoStreamingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseVideos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    VideoId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsPreview = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseVideos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseVideos_Products_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LiveSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    YouTubeLiveVideoId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecordingVideoId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    ScheduledStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsRecordedAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveSessions_Products_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VideoProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    VideoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WatchedSeconds = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoProgress_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoProgress_CourseVideos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "CourseVideos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoAccessLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    VideoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VideoType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoAccessLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoAccessLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoAccessLogs_CourseVideos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "CourseVideos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VideoAccessLogs_LiveSessions_VideoId",
                        column: x => x.VideoId,
                        principalTable: "LiveSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VideoAccessLogs_Products_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseVideos_CourseId",
                table: "CourseVideos",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseVideos_IsPreview",
                table: "CourseVideos",
                column: "IsPreview");

            migrationBuilder.CreateIndex(
                name: "IX_CourseVideos_OrderIndex",
                table: "CourseVideos",
                column: "OrderIndex");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessions_CourseId",
                table: "LiveSessions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessions_IsActive",
                table: "LiveSessions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessions_ScheduledEnd",
                table: "LiveSessions",
                column: "ScheduledEnd");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessions_ScheduledStart",
                table: "LiveSessions",
                column: "ScheduledStart");

            migrationBuilder.CreateIndex(
                name: "IX_VideoAccessLogs_AccessedAt",
                table: "VideoAccessLogs",
                column: "AccessedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VideoAccessLogs_CourseId",
                table: "VideoAccessLogs",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoAccessLogs_IpAddress",
                table: "VideoAccessLogs",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_VideoAccessLogs_UserId",
                table: "VideoAccessLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoAccessLogs_VideoId",
                table: "VideoAccessLogs",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoProgress_UserId",
                table: "VideoProgress",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoProgress_UserId_VideoId",
                table: "VideoProgress",
                columns: new[] { "UserId", "VideoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoProgress_VideoId",
                table: "VideoProgress",
                column: "VideoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoAccessLogs");

            migrationBuilder.DropTable(
                name: "VideoProgress");

            migrationBuilder.DropTable(
                name: "LiveSessions");

            migrationBuilder.DropTable(
                name: "CourseVideos");
        }
    }
}
