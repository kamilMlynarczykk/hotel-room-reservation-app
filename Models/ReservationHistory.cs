using HotelReservationApp.Data;
using System.ComponentModel.DataAnnotations;

namespace HotelReservationApp.Models
{
    public class ReservationHistory
    {
        [Key]
        public int ReservationHistoryID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime AddedDate { get; set; }
        public string Status { get; set; } = StatusPhases.Archived;

        // Foreign keys
        public int ReservationID { get; set; }
        public string EmailAddress { get; set; }
        public string RoomNumber { get; set; }
    }
}
