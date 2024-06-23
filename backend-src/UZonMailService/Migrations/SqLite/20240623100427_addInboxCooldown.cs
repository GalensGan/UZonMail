using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UZonMailService.Migrations.SqLite
{
    /// <inheritdoc />
    public partial class addInboxCooldown : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoxType",
                table: "Outboxes");

            migrationBuilder.RenameColumn(
                name: "BoxType",
                table: "Inboxes",
                newName: "OrganizationId");

            migrationBuilder.AddColumn<int>(
                name: "MinInboxCooldownHours",
                table: "UserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "OrganizationId",
                table: "SendingItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ToEmails",
                table: "SendingItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MinInboxCooldownHours",
                table: "Inboxes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "EmailGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SendingItemInboxes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SendingItemId = table.Column<long>(type: "INTEGER", nullable: false),
                    InboxId = table.Column<long>(type: "INTEGER", nullable: false),
                    ToEmail = table.Column<string>(type: "TEXT", nullable: false),
                    FromEmail = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    SendDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    OrganizationId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendingItemInboxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SendingItemInboxes_Inboxes_InboxId",
                        column: x => x.InboxId,
                        principalTable: "Inboxes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SendingItemInboxes_SendingItems_SendingItemId",
                        column: x => x.SendingItemId,
                        principalTable: "SendingItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Outboxes_Email",
                table: "Outboxes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inboxes_Email",
                table: "Inboxes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SendingItemInboxes_InboxId",
                table: "SendingItemInboxes",
                column: "InboxId");

            migrationBuilder.CreateIndex(
                name: "IX_SendingItemInboxes_SendingItemId",
                table: "SendingItemInboxes",
                column: "SendingItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SendingItemInboxes");

            migrationBuilder.DropIndex(
                name: "IX_Outboxes_Email",
                table: "Outboxes");

            migrationBuilder.DropIndex(
                name: "IX_Inboxes_Email",
                table: "Inboxes");

            migrationBuilder.DropColumn(
                name: "MinInboxCooldownHours",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "SendingItems");

            migrationBuilder.DropColumn(
                name: "ToEmails",
                table: "SendingItems");

            migrationBuilder.DropColumn(
                name: "MinInboxCooldownHours",
                table: "Inboxes");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "EmailGroups");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "Inboxes",
                newName: "BoxType");

            migrationBuilder.AddColumn<int>(
                name: "BoxType",
                table: "Outboxes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
