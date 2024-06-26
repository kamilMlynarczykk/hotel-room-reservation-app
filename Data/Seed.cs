using System;
using System.Linq;
using System.Net;
using HotelReservationApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelReservationApp.Data
{
    public static class Seed
    {
        public static void SeedData(IServiceProvider serviceProvider)
        {
            using (var context = new HotelReservationAppContext(
                serviceProvider.GetRequiredService<DbContextOptions<HotelReservationAppContext>>()))
            {
                // Look for any Room already in the database.
                if (context.Room.Any())
                {
                    return;   // DB has been seeded
                }

                // Define photo directory and file paths
                var photoDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/rooms");
                var photoPath1 = Path.Combine(photoDirectory, "room101.jpg");
                var photoPath2 = Path.Combine(photoDirectory, "room102.jpg");

                // Ensure the directory exists
                if (!Directory.Exists(photoDirectory))
                {
                    Directory.CreateDirectory(photoDirectory);
                }

                // Read photos as byte arrays
                byte[] photo1 = File.Exists(photoPath1) ? File.ReadAllBytes(photoPath1) : null;
                byte[] photo2 = File.Exists(photoPath2) ? File.ReadAllBytes(photoPath2) : null;

                // Seed RoomContents
                var RoomContents1 = new RoomContents
                {
                    Chairs = 2,
                    Beds = 1,
                    Desks = 1,
                    Balconies = 1,
                    TV = 1,
                    Fridge = 1,
                    Kettle = 1
                };

                var RoomContents2 = new RoomContents
                {
                    Chairs = 3,
                    Beds = 2,
                    Desks = 2,
                    Balconies = 0,
                    TV = 2,
                    Fridge = 1,
                    Kettle = 1
                };

                context.RoomContents.AddRange(
                    RoomContents1,
                    RoomContents2
                );

                // Seed Room
                context.Room.AddRange(
                    new Room
                    {
                        RoomNumber = "101",
                        RoomType = "Single",
                        PricePerNight = 100m,
                        Capacity = 1,
                        IsAvailable = true,
                        RoomContents = RoomContents1,
                        Photo = photo1
                    },
                    new Room
                    {
                        RoomNumber = "102",
                        RoomType = "Double",
                        PricePerNight = 150m,
                        Capacity = 2,
                        IsAvailable = true,
                        RoomContents = RoomContents2,
                        Photo = photo2
                    }
                );

                context.SaveChanges();
            }
        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                //Users
                // Admin user
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                string adminUserEmail = "admin@gmail.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new AppUser()
                    {
                        UserName = "app-admin",
                        Email = adminUserEmail,
                        EmailConfirmed = true,
                    };
                    await userManager.CreateAsync(newAdminUser, "Haslo1.");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                // User user
                string appUserEmail = "user1@gmail.com";

                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new AppUser()
                    {
                        UserName = "app-user1",
                        Email = appUserEmail,
                        EmailConfirmed = true,
                    };
                    await userManager.CreateAsync(newAppUser, "Haslo1.");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                }

                string appUserEmail2 = "user2@gmail.com";

                var appUser2 = await userManager.FindByEmailAsync(appUserEmail2);
                if (appUser2 == null)
                {
                    var newAppUser = new AppUser()
                    {
                        UserName = "app-user2",
                        Email = appUserEmail2,
                        EmailConfirmed = true,
                    };
                    await userManager.CreateAsync(newAppUser, "Haslo1.");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                }
            }
        }

        public static void SeedReservationHistory(IServiceProvider serviceProvider)
        {
            using (var context = new HotelReservationAppContext(
                serviceProvider.GetRequiredService<DbContextOptions<HotelReservationAppContext>>()))
            {
                {
                    // Ensure the database is created
                    context.Database.EnsureCreated();

                    // Look for any reservation histories already in the database.
                    if (context.ReservationHistories.Any())
                    {
                        return;   // DB has been seeded
                    }

                    var random = new Random();
                    var reservationHistories = new List<ReservationHistory>();
                    var reservationID = 1;

                    string[] rooms = { "101", "102" };
                    string[] emails = { "user1@gmail.com", "user2@gmail.com" };

                    for (int year = 2018; year <= 2023; year++)
                    {
                        for (int month = 1; month <= 12; month++)
                        {
                            int daysInMonth = DateTime.DaysInMonth(year, month);
                            int numberOfReservations = random.Next(2, 5); // Generating 2 to 4 reservations per month

                            for (int i = 0; i < numberOfReservations; i++)
                            {
                                int startDay = random.Next(1, daysInMonth - 5);
                                DateTime reservationStartDate = new DateTime(year, month, startDay);
                                DateTime reservationEndDate = reservationStartDate.AddDays(random.Next(1, 5));

                                if (reservationEndDate.Month != month)
                                {
                                    reservationEndDate = new DateTime(year, month, daysInMonth);
                                }

                                string room = rooms[random.Next(rooms.Length)];
                                string email = emails[random.Next(emails.Length)];

                                // Check if the room is available
                                bool isRoomAvailable = !reservationHistories.Any(r =>
                                    r.RoomNumber == room &&
                                    ((reservationStartDate >= r.StartDate && reservationStartDate <= r.EndDate) ||
                                     (reservationEndDate >= r.StartDate && reservationEndDate <= r.EndDate) ||
                                     (reservationStartDate <= r.StartDate && reservationEndDate >= r.EndDate)));

                                if (isRoomAvailable)
                                {
                                    reservationHistories.Add(new ReservationHistory
                                    {
                                        StartDate = reservationStartDate,
                                        EndDate = reservationEndDate,
                                        AddedDate = DateTime.Now,
                                        Status = "Archived",
                                        ReservationID = reservationID,
                                        EmailAddress = email,
                                        RoomNumber = room
                                    });

                                    reservationID++;
                                }
                            }
                        }
                    }

                    context.ReservationHistories.AddRange(reservationHistories);
                    context.SaveChanges();
                }
            }
        }
    }
}
