using HotelReservationApp.Models;

public class RoomReservationViewModel
{
    public Room Room { get; set; }
    public Reservation Reservation { get; set; }
    public AppUser AppUser { get; set; }

    public RoomReservationViewModel()
    {
        Room = new Room();
        Reservation = new Reservation();
        AppUser = new AppUser(); 
    }
}
