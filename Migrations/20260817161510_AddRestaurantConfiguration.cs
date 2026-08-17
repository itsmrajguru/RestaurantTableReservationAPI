using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantTableReservationAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestaurantConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaxPartySize = table.Column<int>(type: "int", nullable: false),
                    CancellationWindowHours = table.Column<int>(type: "int", nullable: false),
                    AdvanceBookingDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantConfigurations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestaurantConfigurations");
        }
    }
}
