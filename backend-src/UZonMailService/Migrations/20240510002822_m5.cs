using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UZonMailService.Migrations
{
    /// <inheritdoc />
    public partial class m5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "SendingItems");

            migrationBuilder.DropColumn(
                name: "FailedReason",
                table: "SendingItems");

            migrationBuilder.DropColumn(
                name: "FromEmail",
                table: "SendingItems");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "SendingItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "SendingItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailedReason",
                table: "SendingItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromEmail",
                table: "SendingItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "SendingItems",
                type: "TEXT",
                nullable: true);
        }
    }
}
