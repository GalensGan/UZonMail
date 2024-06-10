using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UZonMailService.Migrations
{
    /// <inheritdoc />
    public partial class m8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachments",
                table: "SendingItem2s");

            migrationBuilder.DropColumn(
                name: "BCC",
                table: "SendingItem2s");

            migrationBuilder.DropColumn(
                name: "CC",
                table: "SendingItem2s");

            migrationBuilder.DropColumn(
                name: "Inboxes",
                table: "SendingItem2s");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Attachments",
                table: "SendingItem2s",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BCC",
                table: "SendingItem2s",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CC",
                table: "SendingItem2s",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inboxes",
                table: "SendingItem2s",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
