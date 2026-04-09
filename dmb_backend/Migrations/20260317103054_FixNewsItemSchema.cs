using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dmb_backend.Migrations
{
    /// <inheritdoc />
    public partial class FixNewsItemSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_news_items_AspNetUsers_CreatedByUserId",
                table: "news_items");

            migrationBuilder.DropIndex(
                name: "IX_news_items_CreatedAtUtc",
                table: "news_items");

            migrationBuilder.DropIndex(
                name: "IX_news_items_CreatedByUserId",
                table: "news_items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_news_items_CreatedAtUtc",
                table: "news_items",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_news_items_CreatedByUserId",
                table: "news_items",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_news_items_AspNetUsers_CreatedByUserId",
                table: "news_items",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
