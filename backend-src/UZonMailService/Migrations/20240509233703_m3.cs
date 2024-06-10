using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UZonMailService.Migrations
{
    /// <inheritdoc />
    public partial class m3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileUsages_SendingItems_SendingItemId",
                table: "FileUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_Inboxes_UserProxies_ProxyId",
                table: "Inboxes");

            migrationBuilder.DropForeignKey(
                name: "FK_SendingItems_SendingGroups_SendingGroupId",
                table: "SendingItems");

            migrationBuilder.DropIndex(
                name: "IX_SendingItems_SendingGroupId",
                table: "SendingItems");

            migrationBuilder.DropIndex(
                name: "IX_Inboxes_ProxyId",
                table: "Inboxes");

            migrationBuilder.DropIndex(
                name: "IX_FileUsages_SendingItemId",
                table: "FileUsages");

            migrationBuilder.DropColumn(
                name: "SendingItemId",
                table: "FileUsages");

            migrationBuilder.AddColumn<string>(
                name: "Attachments",
                table: "SendingItems",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachments",
                table: "SendingItems");

            migrationBuilder.AddColumn<int>(
                name: "SendingItemId",
                table: "FileUsages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SendingItems_SendingGroupId",
                table: "SendingItems",
                column: "SendingGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Inboxes_ProxyId",
                table: "Inboxes",
                column: "ProxyId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUsages_SendingItemId",
                table: "FileUsages",
                column: "SendingItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUsages_SendingItems_SendingItemId",
                table: "FileUsages",
                column: "SendingItemId",
                principalTable: "SendingItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inboxes_UserProxies_ProxyId",
                table: "Inboxes",
                column: "ProxyId",
                principalTable: "UserProxies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SendingItems_SendingGroups_SendingGroupId",
                table: "SendingItems",
                column: "SendingGroupId",
                principalTable: "SendingGroups",
                principalColumn: "Id");
        }
    }
}
