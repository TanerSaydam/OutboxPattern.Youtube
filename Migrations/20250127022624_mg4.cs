using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutboxPattern.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class mg4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderOutboxes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsFailed = table.Column<bool>(type: "bit", nullable: false),
                    FailMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompleteDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderOutboxes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderOutboxes");
        }
    }
}
