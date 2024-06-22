using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UZonMailService.Migrations
{
    /// <inheritdoc />
    public partial class addFileReaderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileReaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileObjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileReaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileReaders_FileObjects_FileObjectId",
                        column: x => x.FileObjectId,
                        principalTable: "FileObjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileReaders_FileObjectId",
                table: "FileReaders",
                column: "FileObjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileReaders");
        }
    }
}
