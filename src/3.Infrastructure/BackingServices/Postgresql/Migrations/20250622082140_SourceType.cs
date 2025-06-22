using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PocLineAPI.Infrastructure.BackingServices.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class SourceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "WebhookEvents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceType",
                table: "WebhookEvents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "WebhookEvents",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "WebhookEvents");

            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "WebhookEvents");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WebhookEvents");
        }
    }
}
