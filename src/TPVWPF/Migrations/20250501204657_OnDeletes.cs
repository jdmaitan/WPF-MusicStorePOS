using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TPVWPF.Migrations
{
    /// <inheritdoc />
    public partial class OnDeletes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketLines_Products_ProductId",
                table: "TicketLines");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketLines_Products_ProductId",
                table: "TicketLines",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketLines_Products_ProductId",
                table: "TicketLines");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketLines_Products_ProductId",
                table: "TicketLines",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
