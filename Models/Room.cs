using System.ComponentModel.DataAnnotations;

namespace HotelReservationApp.Models
{
    public class Room
    {
        [Key]
        public int RoomID { get; set; }

        [Required(ErrorMessage = "Room number is required")]
        [StringLength(10, ErrorMessage = "Room number cannot be longer than 10 characters")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Room type is required")]
        [StringLength(50, ErrorMessage = "Room type cannot be longer than 50 characters")]
        public string RoomType { get; set; }

        [Required(ErrorMessage = "Price per night is required")]
        [Range(0, 10000, ErrorMessage = "Price per night must be between 0 and 10000")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid currency format")]
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
        public int Capacity { get; set; }

        public bool IsAvailable { get; set; }

        public RoomContents RoomContents { get; set; }

        public ICollection<Reservation>? Reservations { get; set; }

        public byte[]? Photo { get; set; }

        public Room()
        {
            RoomContents = new RoomContents();
        }
    }
}
