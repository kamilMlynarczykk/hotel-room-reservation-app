using HotelReservationApp.Data;
using HotelReservationApp.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Reservation
{
    [Key]
    public int ReservationID { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format")]
    [FutureOrPresentDate(ErrorMessage = "Nieprawidołowa data zakwaterowania")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format")]
    [DateGreaterThan("StartDate", ErrorMessage = "Nieprawidłowa data wykwaterowania, należy wybrać datę późniejszą niż data zakwaterowania")]
    public DateTime EndDate { get; set; }
    public DateTime AddedDate { get; set; }
    public string Status { get; set; } = StatusPhases.Pending;

    // Foreign keys
    public string? AppUserID { get; set; }
    public int? RoomID { get; set; }

    public AppUser? AppUser { get; set; }
    public Room? Room { get; set; } 
}
