using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UZonMailService.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inboxes_UserProxies_SystemProxyId",
                table: "Inboxes");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "UserProxies");

            migrationBuilder.DropColumn(
                name: "IgnoreUserIds",
                table: "UserProxies");

            migrationBuilder.DropColumn(
                name: "IsShared",
                table: "UserProxies");

            migrationBuilder.DropColumn(
                name: "MachedUserIds",
                table: "UserProxies");

            migrationBuilder.RenameColumn(
                name: "SystemProxyId",
                table: "Inboxes",
                newName: "ProxyId");

            migrationBuilder.RenameIndex(
                name: "IX_Inboxes_SystemProxyId",
                table: "Inboxes",
                newName: "IX_Inboxes_ProxyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inboxes_UserProxies_ProxyId",
                table: "Inboxes",
                column: "ProxyId",
                principalTable: "UserProxies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inboxes_UserProxies_ProxyId",
                table: "Inboxes");

            migrationBuilder.RenameColumn(
                name: "ProxyId",
                table: "Inboxes",
                newName: "SystemProxyId");

            migrationBuilder.RenameIndex(
                name: "IX_Inboxes_ProxyId",
                table: "Inboxes",
                newName: "IX_Inboxes_SystemProxyId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "UserProxies",
                type: "TEXT",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IgnoreUserIds",
                table: "UserProxies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                table: "UserProxies",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MachedUserIds",
                table: "UserProxies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Inboxes_UserProxies_SystemProxyId",
                table: "Inboxes",
                column: "SystemProxyId",
                principalTable: "UserProxies",
                principalColumn: "Id");
        }
    }
}
