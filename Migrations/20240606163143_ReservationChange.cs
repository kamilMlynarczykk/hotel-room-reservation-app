using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelReservationApp.Migrations
{
    /// <inheritdoc />
    public partial class ReservationChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_AppUserID",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Room_RoomID",
                table: "Reservations");

            migrationBuilder.AlterColumn<int>(
                name: "RoomID",
                table: "Reservations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserID",
                table: "Reservations",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_AppUserID",
                table: "Reservations",
                column: "AppUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Room_RoomID",
                table: "Reservations",
                column: "RoomID",
                principalTable: "Room",
                principalColumn: "RoomID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_AppUserID",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Room_RoomID",
                table: "Reservations");

            migrationBuilder.AlterColumn<int>(
                name: "RoomID",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AppUserID",
                table: "Reservations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_AppUserID",
                table: "Reservations",
                column: "AppUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Room_RoomID",
                table: "Reservations",
                column: "RoomID",
                principalTable: "Room",
                principalColumn: "RoomID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
