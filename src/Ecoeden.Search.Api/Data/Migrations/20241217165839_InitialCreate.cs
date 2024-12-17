using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecoeden.Search.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ecoeden.event");

            migrationBuilder.CreateTable(
                name: "EventPublishHistories",
                schema: "ecoeden.event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    FailureSource = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventStatus = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPublishHistories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventPublishHistories",
                schema: "ecoeden.event");
        }
    }
}
