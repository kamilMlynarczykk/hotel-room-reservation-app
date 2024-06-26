using HotelReservationApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelReservationApp.Models
{
    public class RoomReservationStatsViewModel
    {
        // Dropdowns
        public List<int> Years { get; set; }
        public List<SelectListItem> Months { get; set; }
        public List<string> RoomNumbers { get; set; }

        // Selected values
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }
        public string SelectedRoomNumber { get; set; }

        // Statistics data
        public List<ChartDataViewModel> ReservationStats { get; set; }
    }

    public class ReservationStats
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int ReservationCount { get; set; }
    }
}
