using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlbionKillnet.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KillEvents",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    BattleId = table.Column<long>(type: "INTEGER", nullable: false),
                    ServerRegion = table.Column<string>(type: "TEXT", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalVictimKillFame = table.Column<int>(type: "INTEGER", nullable: false),
                    KillArea = table.Column<string>(type: "TEXT", nullable: true),
                    KillerName = table.Column<string>(type: "TEXT", nullable: false),
                    VictimName = table.Column<string>(type: "TEXT", nullable: false),
                    RawData = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KillEvents", x => x.EventId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KillEvents");
        }
    }
}
