using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelReservationApp.Data;
using HotelReservationApp.Models;
using Microsoft.AspNetCore.Authorization;
using NuGet.Protocol;
using System.Security.Claims;
using System.Data;
using Microsoft.AspNetCore.Identity;
using HotelReservationApp.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HotelReservationApp.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly HotelReservationAppContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ReservationsController(HotelReservationAppContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservations
        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        public async Task<IActionResult> Index(string statusFilter, string emailFilter, string roomFilter, string sortBy)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("admin");
            List<string> roomNumbers;

            if (isAdmin)
            {
                // For admins, get all room numbers
                roomNumbers = await _context.Room.Select(r => r.RoomNumber).ToListAsync();
            }
            else
            {
                // For regular users, get only their room numbers
                roomNumbers = await _context.Reservations
                                            .Where(r => r.AppUserID == userId)
                                            .Select(r => r.Room.RoomNumber)
                                            .Distinct()
                                            .ToListAsync();
            }

            // Pass the room numbers to the view via ViewBag
            ViewBag.RoomNumbers = new SelectList(roomNumbers.Select(rn => new SelectListItem { Value = rn.ToString(), Text = rn.ToString() }), "Value", "Text");

            var userEmails = await _context.Users.Select(u => u.Email).ToListAsync();
            ViewBag.UserEmails = new SelectList(userEmails.Select(ue => new SelectListItem { Value = ue.ToString(), Text = ue.ToString() }), "Value", "Text");

            var reservationsQuery = _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.AppUser)
                .AsQueryable();

            if (!isAdmin)
            {
                reservationsQuery = reservationsQuery.Where(r => r.AppUserID == userId);
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                reservationsQuery = reservationsQuery.Where(r => r.Status == statusFilter);
            }

            // Filter by email
            if (!string.IsNullOrEmpty(emailFilter))
            {
                reservationsQuery = reservationsQuery.Where(r => r.AppUser.Email.Contains(emailFilter));
            }

            // Filter by room number
            if (!string.IsNullOrEmpty(roomFilter))
            {
                reservationsQuery = reservationsQuery.Where(r => r.Room.RoomNumber.ToString().Contains(roomFilter));
            }

            switch (sortBy)
            {
                case "StartDate":
                    reservationsQuery = reservationsQuery.OrderBy(r => r.StartDate);
                    break;
                case "EndDate":
                    reservationsQuery = reservationsQuery.OrderBy(r => r.EndDate);
                    break;
                case "AddedDate":
                    reservationsQuery = reservationsQuery.OrderBy(r => r.AddedDate);
                    break;
                case "StartDate_desc":
                    reservationsQuery = reservationsQuery.OrderByDescending(r => r.StartDate);
                    break;
                case "EndDate_desc":
                    reservationsQuery = reservationsQuery.OrderByDescending(r => r.EndDate);
                    break;
                case "AddedDate_desc":
                    reservationsQuery = reservationsQuery.OrderByDescending(r => r.AddedDate);
                    break;
                default:
                    // Default sorting logic, you can change this as needed
                    reservationsQuery = reservationsQuery.OrderBy(r => r.StartDate);
                    break;
            }
            return View(reservationsQuery.ToList());
        }


        // GET: Reservations/Details/5
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.ReservationID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        public IActionResult Create(int RoomID)
        {
            ViewData["RoomID"] = RoomID;

            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        public async Task<IActionResult> Create(RoomReservationViewModel roomReservationViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewData["RoomID"] = new SelectList(_context.Room, "RoomID", "RoomNumber", roomReservationViewModel.Room.RoomID);
                ViewData["AppUserID"] = new SelectList(_context.AppUsers, "Id", "UserName", roomReservationViewModel.Reservation.AppUserID);
                return View(roomReservationViewModel);
            }

            var room = _context.Room
                    .Include(r => r.RoomContents)
                    .FirstOrDefault(r => r.RoomNumber == roomReservationViewModel.Room.RoomNumber);

            if (room == null)
            {
                ModelState.AddModelError("", "Nie ma pokoju o takim numerze.");
                ViewData["RoomID"] = new SelectList(_context.Room, "RoomID", "RoomNumber", roomReservationViewModel.Room.RoomID);
                ViewData["AppUserID"] = new SelectList(_context.AppUsers, "Id", "UserName", roomReservationViewModel.Reservation.AppUserID);
                return View(roomReservationViewModel);
            }

            var appUser = _context.Users.FirstOrDefault(u => u.Email == roomReservationViewModel.AppUser.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Nie ma zarejestrowanego użytkownika o podanym adresie email.");
                ViewData["RoomID"] = new SelectList(_context.Room, "RoomID", "RoomNumber", roomReservationViewModel.Room.RoomID);
                ViewData["AppUserID"] = new SelectList(_context.AppUsers, "Id", "UserName", roomReservationViewModel.Reservation.AppUserID);
                return View(roomReservationViewModel);
            }

            roomReservationViewModel.AppUser.Id = appUser.Id;
            roomReservationViewModel.Room.RoomID = room.RoomID;

            bool datesConflict = _context.Reservations
                .Where(r => r.RoomID == roomReservationViewModel.Room.RoomID)
                .Any(r => r.StartDate < roomReservationViewModel.Reservation.EndDate && r.EndDate > roomReservationViewModel.Reservation.StartDate);

            if (datesConflict)
            {
                ModelState.AddModelError("", "Pokój jest zarezerwowany dla podanej daty, spróbuj innych dat");
                ViewData["RoomID"] = new SelectList(_context.Room, "RoomID", "RoomNumber", roomReservationViewModel.Room.RoomID);
                ViewData["AppUserID"] = new SelectList(_context.AppUsers, "Id", "UserName", roomReservationViewModel.Reservation.AppUserID);
                return View(roomReservationViewModel);
            }

            var reservation = new Reservation()
            {
                StartDate = roomReservationViewModel.Reservation.StartDate,
                EndDate = roomReservationViewModel.Reservation.EndDate,
                AddedDate = DateTime.Now,
                Status = StatusPhases.Accepted,
                AppUserID = appUser.Id,
                RoomID = room.RoomID,
                AppUser = appUser,
                Room = room,
            };

            _context.Add(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Reservations/Edit/5
        [HttpGet]
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.User)]
        public async Task<IActionResult> Edit(int id)

        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.Room = await _context.Room
                .Include(r => r.RoomContents)
                .FirstOrDefaultAsync(r => r.RoomID == reservation.RoomID);

            if (reservation.Room == null)
            {
                return NotFound();
            }

            reservation.AppUser = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.Id == reservation.AppUserID);

            if (reservation.AppUser == null)
            {
                return NotFound();
            }

            return View("Edit", reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.User)]
        public async Task<IActionResult> Edit(Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return View(reservation);
            }

            var room = await _context.Room
                .Include(r => r.RoomContents)
                .FirstOrDefaultAsync(r => r.RoomNumber == reservation.Room.RoomNumber);

            if (room == null)
            {
                ModelState.AddModelError("", "Nie ma pokoju o takim numerze.");
                return View(reservation);
            }

            bool datesConflict = await _context.Reservations
                .Where(r => r.RoomID == room.RoomID)
                .Where(r => r.ReservationID != reservation.ReservationID)
                .AnyAsync(r => r.StartDate < reservation.EndDate && r.EndDate > reservation.StartDate);

            if (datesConflict)
            {
                ModelState.AddModelError("", "Pokój jest zarezerwowany dla podanej daty, spróbuj innych dat");
                return View(reservation);
            }

            var existingReservation = await _context.Reservations.FindAsync(reservation.ReservationID);

            if (existingReservation == null)
            {
                return NotFound();
            }

            // Update specific properties
            existingReservation.StartDate = reservation.StartDate;
            existingReservation.EndDate = reservation.EndDate;
            existingReservation.RoomID = room.RoomID;
            existingReservation.Status = reservation.Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(reservation.ReservationID))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", "Wystąpił błąd podczas zapisywania zmian. Spróbuj ponownie.");
                    return View(reservation);
                }
            }

            return RedirectToAction("Index");
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationID == id);
        }



        // GET: Reservations/Delete/5
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.ReservationID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Accept(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.Status = StatusPhases.Accepted;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Deny(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.Status = StatusPhases.Denied;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
