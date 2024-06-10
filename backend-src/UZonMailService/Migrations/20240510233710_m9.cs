using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UZonMailService.Migrations
{
    /// <inheritdoc />
    public partial class m9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "SendingItem2s",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailedReason",
                table: "SendingItem2s",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromEmail",
                table: "SendingItem2s",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "SendingItem2s",
                type: "TEXT",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileUsages_SendingItem2s_SendingItem2Id",
                table: "FileUsages");

            migrationBuilder.DropIndex(
                name: "IX_FileUsages_SendingItem2Id",
                table: "FileUsages");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "SendingItem2s");

            migrationBuilder.DropColumn(
                name: "FailedReason",
                table: "SendingItem2s");

            migrationBuilder.DropColumn(
                name: "FromEmail",
                table: "SendingItem2s");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "SendingItem2s");

            migrationBuilder.DropColumn(
                name: "SendingItem2Id",
                table: "FileUsages");
        }
    }
}
