using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelReservationApp.Migrations
{
    /// <inheritdoc />
    public partial class resHisChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppUserID",
                table: "ReservationHistories");

            migrationBuilder.DropColumn(
                name: "RoomID",
                table: "ReservationHistories");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "ReservationHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoomNumber",
                table: "ReservationHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "ReservationHistories");

            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "ReservationHistories");

            migrationBuilder.AddColumn<string>(
                name: "AppUserID",
                table: "ReservationHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomID",
                table: "ReservationHistories",
                type: "int",
                nullable: true);
        }
    }
}
