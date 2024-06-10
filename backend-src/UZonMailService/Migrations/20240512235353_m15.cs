using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UZonMailService.Migrations
{
    /// <inheritdoc />
    public partial class m15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplateSendingGroup_EmailTemplates_TemplatesId",
                table: "EmailTemplateSendingGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplateSendingGroup_SendingGroups_SendingGroupsId",
                table: "EmailTemplateSendingGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_OutboxSendingGroup_Inboxes_OutboxesId",
                table: "OutboxSendingGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_OutboxSendingGroup_SendingGroups_SendingGroupsId",
                table: "OutboxSendingGroup");

            migrationBuilder.DropTable(
                name: "EmailAddress");

            migrationBuilder.DropTable(
                name: "SendingItem2s");

            migrationBuilder.DropColumn(
                name: "Attachments",
                table: "SendingGroups");

            migrationBuilder.RenameColumn(
                name: "Attachments",
                table: "SendingItems",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "SendingGroupsId",
                table: "OutboxSendingGroup",
                newName: "SendingGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxSendingGroup_SendingGroupsId",
                table: "OutboxSendingGroup",
                newName: "IX_OutboxSendingGroup_SendingGroupId");

            migrationBuilder.RenameColumn(
                name: "SendingGroupsId",
                table: "EmailTemplateSendingGroup",
                newName: "SendingGroupId");

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

            migrationBuilder.CreateTable(
                name: "FileUsageSendingGroup",
                columns: table => new
                {
                    AttachmentsId = table.Column<int>(type: "INTEGER", nullable: false),
                    SendingGroupId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUsageSendingGroup", x => new { x.AttachmentsId, x.SendingGroupId });
                    table.ForeignKey(
                        name: "FK_FileUsageSendingGroup_FileUsages_AttachmentsId",
                        column: x => x.AttachmentsId,
                        principalTable: "FileUsages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileUsageSendingGroup_SendingGroups_SendingGroupId",
                        column: x => x.SendingGroupId,
                        principalTable: "SendingGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileUsageSendingItem",
                columns: table => new
                {
                    AttachmentsId = table.Column<int>(type: "INTEGER", nullable: false),
                    SendingItemId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUsageSendingItem", x => new { x.AttachmentsId, x.SendingItemId });
                    table.ForeignKey(
                        name: "FK_FileUsageSendingItem_FileUsages_AttachmentsId",
                        column: x => x.AttachmentsId,
                        principalTable: "FileUsages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileUsageSendingItem_SendingItems_SendingItemId",
                        column: x => x.SendingItemId,
                        principalTable: "SendingItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileUsageSendingGroup_SendingGroupId",
                table: "FileUsageSendingGroup",
                column: "SendingGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUsageSendingItem_SendingItemId",
                table: "FileUsageSendingItem",
                column: "SendingItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplateSendingGroup_EmailTemplates_TemplatesId",
                table: "EmailTemplateSendingGroup",
                column: "TemplatesId",
                principalTable: "EmailTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplateSendingGroup_SendingGroups_SendingGroupId",
                table: "EmailTemplateSendingGroup",
                column: "SendingGroupId",
                principalTable: "SendingGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxSendingGroup_Inboxes_OutboxesId",
                table: "OutboxSendingGroup",
                column: "OutboxesId",
                principalTable: "Inboxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxSendingGroup_SendingGroups_SendingGroupId",
                table: "OutboxSendingGroup",
                column: "SendingGroupId",
                principalTable: "SendingGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplateSendingGroup_EmailTemplates_TemplatesId",
                table: "EmailTemplateSendingGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplateSendingGroup_SendingGroups_SendingGroupId",
                table: "EmailTemplateSendingGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_OutboxSendingGroup_Inboxes_OutboxesId",
                table: "OutboxSendingGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_OutboxSendingGroup_SendingGroups_SendingGroupId",
                table: "OutboxSendingGroup");

            migrationBuilder.DropTable(
                name: "FileUsageSendingGroup");

            migrationBuilder.DropTable(
                name: "FileUsageSendingItem");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "SendingItems");

            migrationBuilder.DropColumn(
                name: "FailedReason",
                table: "SendingItems");

            migrationBuilder.DropColumn(
                name: "FromEmail",
                table: "SendingItems");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "SendingItems",
                newName: "Attachments");

            migrationBuilder.RenameColumn(
                name: "SendingGroupId",
                table: "OutboxSendingGroup",
                newName: "SendingGroupsId");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxSendingGroup_SendingGroupId",
                table: "OutboxSendingGroup",
                newName: "IX_OutboxSendingGroup_SendingGroupsId");

            migrationBuilder.RenameColumn(
                name: "SendingGroupId",
                table: "EmailTemplateSendingGroup",
                newName: "SendingGroupsId");

            migrationBuilder.AddColumn<string>(
                name: "Attachments",
                table: "SendingGroups",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmailAddress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAddress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SendingItem2s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Attachments = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmailTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    FailedReason = table.Column<string>(type: "TEXT", nullable: true),
                    FromEmail = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSendingBatch = table.Column<bool>(type: "INTEGER", nullable: false),
                    OutBoxId = table.Column<int>(type: "INTEGER", nullable: false),
                    SendingGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    TriedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendingItem2s", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplateSendingGroup_EmailTemplates_TemplatesId",
                table: "EmailTemplateSendingGroup",
                column: "TemplatesId",
                principalTable: "EmailTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplateSendingGroup_SendingGroups_SendingGroupsId",
                table: "EmailTemplateSendingGroup",
                column: "SendingGroupsId",
                principalTable: "SendingGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxSendingGroup_Inboxes_OutboxesId",
                table: "OutboxSendingGroup",
                column: "OutboxesId",
                principalTable: "Inboxes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxSendingGroup_SendingGroups_SendingGroupsId",
                table: "OutboxSendingGroup",
                column: "SendingGroupsId",
                principalTable: "SendingGroups",
                principalColumn: "Id");
        }
    }
}
