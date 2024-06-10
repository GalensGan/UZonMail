using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UZonMailService.Migrations
{
    /// <inheritdoc />
    public partial class _060202 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OutboxCooldownMs",
                table: "UserSettings",
                newName: "MinOutboxCooldownSecond");

            migrationBuilder.AddColumn<int>(
                name: "MaxOutboxCooldownSecond",
                table: "UserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxSendingBatchSize",
                table: "UserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProxyId",
                table: "SendingItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "SendingGroups",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "LastMessage",
                table: "SendingGroups",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxOutboxCooldownSecond",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "MaxSendingBatchSize",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "ProxyId",
                table: "SendingItems");

            migrationBuilder.DropColumn(
                name: "LastMessage",
                table: "SendingGroups");

            migrationBuilder.RenameColumn(
                name: "MinOutboxCooldownSecond",
                table: "UserSettings",
                newName: "OutboxCooldownMs");

            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "SendingGroups",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
