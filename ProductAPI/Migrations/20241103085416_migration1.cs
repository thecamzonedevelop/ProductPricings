using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Category = table.Column<byte>(type: "tinyint", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Code", "Created", "LastUpdated", "Name" },
                values: new object[,]
                {
                    { "08802dba-e7db-4084-81a8-e58d5db624c3", (byte)4, "PRD003", new DateTime(2024, 11, 3, 15, 54, 16, 457, DateTimeKind.Local).AddTicks(9906), null, "TShirt-SEA game 2023" },
                    { "82124f5a-2f0a-438e-82e9-3aa735cd9367", (byte)1, "PRD001", new DateTime(2024, 11, 3, 15, 54, 16, 457, DateTimeKind.Local).AddTicks(9887), null, "Coca" },
                    { "cc92d39e-2de5-4b00-8c54-83dffb2362dc", (byte)32, "PRD002", new DateTime(2024, 11, 3, 15, 54, 16, 457, DateTimeKind.Local).AddTicks(9903), null, "Dream 125" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Code",
                table: "Products",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
