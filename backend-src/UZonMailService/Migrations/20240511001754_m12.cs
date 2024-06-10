using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UZonMailService.Migrations
{
    /// <inheritdoc />
    public partial class m12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileUsages_SendingItem2s_SendingItem2Id",
                table: "FileUsages");

            migrationBuilder.DropIndex(
                name: "IX_FileUsages_SendingItem2Id",
                table: "FileUsages");

            migrationBuilder.DropColumn(
                name: "SendingItem2Id",
                table: "FileUsages");

            migrationBuilder.CreateTable(
                name: "FileUsageSendingItem2",
                columns: table => new
                {
                    AttachmentsId = table.Column<int>(type: "INTEGER", nullable: false),
                    SendingItem2Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUsageSendingItem2", x => new { x.AttachmentsId, x.SendingItem2Id });
                    table.ForeignKey(
                        name: "FK_FileUsageSendingItem2_FileUsages_AttachmentsId",
                        column: x => x.AttachmentsId,
                        principalTable: "FileUsages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileUsageSendingItem2_SendingItem2s_SendingItem2Id",
                        column: x => x.SendingItem2Id,
                        principalTable: "SendingItem2s",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileUsageSendingItem2_SendingItem2Id",
                table: "FileUsageSendingItem2",
                column: "SendingItem2Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileUsageSendingItem2");

            migrationBuilder.AddColumn<int>(
                name: "SendingItem2Id",
                table: "FileUsages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileUsages_SendingItem2Id",
                table: "FileUsages",
                column: "SendingItem2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUsages_SendingItem2s_SendingItem2Id",
                table: "FileUsages",
                column: "SendingItem2Id",
                principalTable: "SendingItem2s",
                principalColumn: "Id");
        }
    }
}
