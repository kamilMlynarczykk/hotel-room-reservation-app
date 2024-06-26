using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class AppUser : IdentityUser
{
    // Navigation property
    public ICollection<Reservation>? Reservations { get; set; }
}
