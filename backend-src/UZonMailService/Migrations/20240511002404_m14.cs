using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UZonMailService.Migrations
{
    /// <inheritdoc />
    public partial class m14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileUsageSendingItem2");

            migrationBuilder.AddColumn<string>(
                name: "Attachments",
                table: "SendingItem2s",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachments",
                table: "SendingItem2s");

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
    }
}
