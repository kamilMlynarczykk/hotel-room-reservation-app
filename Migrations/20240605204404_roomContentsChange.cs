using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelReservationApp.Migrations
{
    /// <inheritdoc />
    public partial class RoomContentsChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_AppUser_CustomerID",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Room_RoomID",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Room_RoomContents_RoomContentsId",
                table: "Room");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservation",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_CustomerID",
                table: "Reservation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppUser",
                table: "AppUser");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Reservation");

            migrationBuilder.RenameTable(
                name: "Reservation",
                newName: "Reservations");

            migrationBuilder.RenameTable(
                name: "AppUser",
                newName: "AppUsers");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_RoomID",
                table: "Reservations",
                newName: "IX_Reservations_RoomID");

            migrationBuilder.AlterColumn<int>(
                name: "RoomContentsId",
                table: "Room",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserID",
                table: "Reservations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations",
                column: "ReservationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppUsers",
                table: "AppUsers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_AppUserID",
                table: "Reservations",
                column: "AppUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AppUsers_AppUserID",
                table: "Reservations",
                column: "AppUserID",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Room_RoomID",
                table: "Reservations",
                column: "RoomID",
                principalTable: "Room",
                principalColumn: "RoomID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Room_RoomContents_RoomContentsId",
                table: "Room",
                column: "RoomContentsId",
                principalTable: "RoomContents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AppUsers_AppUserID",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Room_RoomID",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Room_RoomContents_RoomContentsId",
                table: "Room");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_AppUserID",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppUsers",
                table: "AppUsers");

            migrationBuilder.RenameTable(
                name: "Reservations",
                newName: "Reservation");

            migrationBuilder.RenameTable(
                name: "AppUsers",
                newName: "AppUser");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_RoomID",
                table: "Reservation",
                newName: "IX_Reservation_RoomID");

            migrationBuilder.AlterColumn<int>(
                name: "RoomContentsId",
                table: "Room",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AppUserID",
                table: "Reservation",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "CustomerID",
                table: "Reservation",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservation",
                table: "Reservation",
                column: "ReservationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppUser",
                table: "AppUser",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_CustomerID",
                table: "Reservation",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_AppUser_CustomerID",
                table: "Reservation",
                column: "CustomerID",
                principalTable: "AppUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Room_RoomID",
                table: "Reservation",
                column: "RoomID",
                principalTable: "Room",
                principalColumn: "RoomID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Room_RoomContents_RoomContentsId",
                table: "Room",
                column: "RoomContentsId",
                principalTable: "RoomContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
