using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UZonMailService.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailAddress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAddress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Thumbnail = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileBuckets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BucketName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    RootDir = table.Column<string>(type: "TEXT", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileBuckets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GlobalUserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    OutboxCooldownMs = table.Column<long>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalUserSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissionCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    PermissionCodeId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissionCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SendingGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Subjects = table.Column<string>(type: "TEXT", nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: true),
                    Inboxes = table.Column<string>(type: "TEXT", nullable: false),
                    CcBoxes = table.Column<string>(type: "TEXT", nullable: true),
                    BccBoxes = table.Column<string>(type: "TEXT", nullable: true),
                    Attachments = table.Column<string>(type: "TEXT", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: false),
                    IsDistributed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TotalCount = table.Column<int>(type: "INTEGER", nullable: false),
                    SuccessCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    SendStartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SendEndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SendingType = table.Column<int>(type: "INTEGER", nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendingGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProxies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    MatchRegex = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Proxy = table.Column<string>(type: "TEXT", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    IgnoreUserIds = table.Column<string>(type: "TEXT", nullable: true),
                    MachedUserIds = table.Column<string>(type: "TEXT", nullable: true),
                    IsShared = table.Column<bool>(type: "INTEGER", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProxies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileBucketId = table.Column<int>(type: "INTEGER", nullable: false),
                    LastModifyDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Sha256 = table.Column<string>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    LinkCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Size = table.Column<long>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileObjects_FileBuckets_FileBucketId",
                        column: x => x.FileBucketId,
                        principalTable: "FileBuckets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PermissionCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionCodes_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Avatar = table.Column<string>(type: "TEXT", nullable: true),
                    ConnectionId = table.Column<string>(type: "TEXT", nullable: true),
                    ForbiddenToLogin = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSuperAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSystemUser = table.Column<bool>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplateSendingGroup",
                columns: table => new
                {
                    SendingGroupsId = table.Column<int>(type: "INTEGER", nullable: false),
                    TemplatesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplateSendingGroup", x => new { x.SendingGroupsId, x.TemplatesId });
                    table.ForeignKey(
                        name: "FK_EmailTemplateSendingGroup_EmailTemplates_TemplatesId",
                        column: x => x.TemplatesId,
                        principalTable: "EmailTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmailTemplateSendingGroup_SendingGroups_SendingGroupsId",
                        column: x => x.SendingGroupsId,
                        principalTable: "SendingGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SendingItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SendingGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    OutBoxId = table.Column<int>(type: "INTEGER", nullable: false),
                    FromEmail = table.Column<string>(type: "TEXT", nullable: true),
                    Inboxes = table.Column<string>(type: "TEXT", nullable: false),
                    CC = table.Column<string>(type: "TEXT", nullable: true),
                    BCC = table.Column<string>(type: "TEXT", nullable: true),
                    EmailTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    IsSendingBatch = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    FailedReason = table.Column<string>(type: "TEXT", nullable: true),
                    TriedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SendingItems_SendingGroups_SendingGroupId",
                        column: x => x.SendingGroupId,
                        principalTable: "SendingGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmailGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<long>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailGroups_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FileUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    UniqueName = table.Column<string>(type: "TEXT", nullable: true),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileObjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendingItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileUsages_FileObjects_FileObjectId",
                        column: x => x.FileObjectId,
                        principalTable: "FileObjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileUsages_SendingItems_SendingItemId",
                        column: x => x.SendingItemId,
                        principalTable: "SendingItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileUsages_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Inboxes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmailGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Remark = table.Column<string>(type: "TEXT", nullable: true),
                    BoxType = table.Column<int>(type: "INTEGER", nullable: false),
                    LinkCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    SmtpHost = table.Column<string>(type: "TEXT", nullable: true),
                    SmtpPort = table.Column<int>(type: "INTEGER", nullable: true),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    EnableSSL = table.Column<bool>(type: "INTEGER", nullable: true),
                    SystemProxyId = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxSendCountPerDay = table.Column<int>(type: "INTEGER", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inboxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inboxes_EmailGroups_EmailGroupId",
                        column: x => x.EmailGroupId,
                        principalTable: "EmailGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Inboxes_UserProxies_SystemProxyId",
                        column: x => x.SystemProxyId,
                        principalTable: "UserProxies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OutboxSendingGroup",
                columns: table => new
                {
                    OutboxesId = table.Column<int>(type: "INTEGER", nullable: false),
                    SendingGroupsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxSendingGroup", x => new { x.OutboxesId, x.SendingGroupsId });
                    table.ForeignKey(
                        name: "FK_OutboxSendingGroup_Inboxes_OutboxesId",
                        column: x => x.OutboxesId,
                        principalTable: "Inboxes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OutboxSendingGroup_SendingGroups_SendingGroupsId",
                        column: x => x.SendingGroupsId,
                        principalTable: "SendingGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailGroups_UserId",
                table: "EmailGroups",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplateSendingGroup_TemplatesId",
                table: "EmailTemplateSendingGroup",
                column: "TemplatesId");

            migrationBuilder.CreateIndex(
                name: "IX_FileObjects_FileBucketId",
                table: "FileObjects",
                column: "FileBucketId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUsages_FileObjectId",
                table: "FileUsages",
                column: "FileObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUsages_OwnerUserId",
                table: "FileUsages",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUsages_SendingItemId",
                table: "FileUsages",
                column: "SendingItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Inboxes_EmailGroupId",
                table: "Inboxes",
                column: "EmailGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Inboxes_SystemProxyId",
                table: "Inboxes",
                column: "SystemProxyId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxSendingGroup_SendingGroupsId",
                table: "OutboxSendingGroup",
                column: "SendingGroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionCodes_RoleId",
                table: "PermissionCodes",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SendingItems_SendingGroupId",
                table: "SendingItems",
                column: "SendingGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailAddress");

            migrationBuilder.DropTable(
                name: "EmailTemplateSendingGroup");

            migrationBuilder.DropTable(
                name: "FileUsages");

            migrationBuilder.DropTable(
                name: "GlobalUserSettings");

            migrationBuilder.DropTable(
                name: "OutboxSendingGroup");

            migrationBuilder.DropTable(
                name: "PermissionCodes");

            migrationBuilder.DropTable(
                name: "RolePermissionCodes");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.DropTable(
                name: "FileObjects");

            migrationBuilder.DropTable(
                name: "SendingItems");

            migrationBuilder.DropTable(
                name: "Inboxes");

            migrationBuilder.DropTable(
                name: "FileBuckets");

            migrationBuilder.DropTable(
                name: "SendingGroups");

            migrationBuilder.DropTable(
                name: "EmailGroups");

            migrationBuilder.DropTable(
                name: "UserProxies");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
