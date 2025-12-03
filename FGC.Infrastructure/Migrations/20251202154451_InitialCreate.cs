using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FGC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(2000)", maxLength: 2000, nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    Category = table.Column<int>(type: "INT", nullable: false),
                    Developer = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: false),
                    Publisher = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false, defaultValue: true),
                    Rating = table.Column<double>(type: "FLOAT", nullable: false, defaultValue: 0.0),
                    TotalSales = table.Column<int>(type: "INT", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "INT", nullable: false),
                    Method = table.Column<int>(type: "INT", nullable: false),
                    TransactionId = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    FailureReason = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR(254)", maxLength: 254, nullable: false),
                    Password = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "INT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_Category",
                table: "Games",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Games_IsActive",
                table: "Games",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Price",
                table: "Games",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Title",
                table: "Games",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CreatedAt",
                table: "Payments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_GameId",
                table: "Payments",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionId",
                table: "Payments",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                table: "Users",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
