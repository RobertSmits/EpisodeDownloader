using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VrtNuDownloader.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Downloaded",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    DownloadDate = table.Column<DateTime>(nullable: false),
                    EpisodeUrl = table.Column<string>(nullable: true),
                    VideoUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Downloaded", x => x.Name);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Downloaded");
        }
    }
}
