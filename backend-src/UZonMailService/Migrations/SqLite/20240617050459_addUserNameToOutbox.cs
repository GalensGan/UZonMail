using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UZonMailService.Migrations
{
    /// <inheritdoc />
    public partial class addUserNameToOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Inboxes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Inboxes");
        }
    }
}
