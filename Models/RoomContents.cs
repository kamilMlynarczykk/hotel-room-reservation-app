using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelReservationApp.Models
{
    public class RoomContents
    {
        [Key]
        public int? Id { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Chairs must be a non-negative number")]
        [Required(ErrorMessage = "Chairs is required")]
        public int Chairs { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Beds must be a non-negative number")]
        [Required(ErrorMessage = "Beds is required")]
        public int Beds { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Desks must be a non-negative number")]
        [Required(ErrorMessage = "Desks is required")]
        public int Desks { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Balconies must be a non-negative number")]
        [Required(ErrorMessage = "Balconies is required")]
        public int Balconies { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "TVs must be a non-negative number")]
        [Required(ErrorMessage = "TV is required")]
        public int TV { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Fridges must be a non-negative number")]
        [Required(ErrorMessage = "Fridge is required")]
        public int Fridge { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Kettles must be a non-negative number")]
        [Required(ErrorMessage = "Kettle is required")]
        public int Kettle { get; set; }
    }
}
