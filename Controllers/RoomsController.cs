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
using System.Security.Claims;

namespace HotelReservationApp.Controllers
{
    public class RoomsController : Controller
    {
        private readonly HotelReservationAppContext _context;

        public RoomsController(HotelReservationAppContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var roomsQuery = _context.Room.Include(r => r.Reservations).AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                roomsQuery = roomsQuery.Where(room => !room.Reservations.Any(reservation => reservation.StartDate < endDate && reservation.EndDate > startDate));
            }

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            var rooms = await roomsQuery.ToListAsync();

            // Create the Reservation object with the provided dates
            var reservation = new Reservation
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };

            // Create the tuple with rooms and reservation
            var model = new Tuple<IEnumerable<Room>, Reservation>(rooms, reservation);

            return View(model);
        }

        // GET: Rooms/Details/5
        public IActionResult Details(int id)
        {
            var room = _context.Room
                .Include(r => r.RoomContents)
                .Include(r => r.Reservations)
                .FirstOrDefault(r => r.RoomID == id);

            if (room == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users
                .Include(u => u.Reservations)
                .FirstOrDefault(u => u.Id == userId);

            var viewModel = new RoomReservationViewModel
            {
                Room = room,
                AppUser = user ?? new AppUser(),
            };

            return View("DetailsAndCreateReservation", viewModel); // Ensure correct view name
        }

        // POST: Rooms/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(RoomReservationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ViewBag.AvailableDates = null;

                bool datesConflict = _context.Reservations
                    .Where(r => r.RoomID == viewModel.Room.RoomID)
                    .Any(r => r.StartDate < viewModel.Reservation.EndDate && r.EndDate > viewModel.Reservation.StartDate);

                if (datesConflict)
                {
                    ModelState.AddModelError("", "Ten pokój jest zarezerwowany dla wybranych dat, spróbuj wybrać inne lub dostępne poniżej:");

                    // Get all reservations for the room
                    var reservations = _context.Reservations
                        .Where(r => r.RoomID == viewModel.Room.RoomID && r.EndDate >= DateTime.Today)
                        .OrderBy(r => r.StartDate)
                        .ToList();

                    // List to store upcoming available date ranges
                    var upcomingAvailableDates = new List<(DateTime StartDate, DateTime EndDate)>();

                    // Calculate gaps between reservations
                    DateTime currentDate = DateTime.Today;
                    DateTime nextAvailableStartDate = currentDate;

                    DateTime LastNotAvailableDate = new DateTime();
                    foreach (var reservation in reservations)
                    {
                        if (reservation.StartDate > nextAvailableStartDate)
                        {
                            // Found a gap, add it to upcoming available dates
                            upcomingAvailableDates.Add((nextAvailableStartDate, reservation.StartDate));
                        }

                        // Update next available start date to end of current reservation
                        nextAvailableStartDate = reservation.EndDate;
                        LastNotAvailableDate = nextAvailableStartDate;
                    }

                    // Check if there's a gap from the last reservation to today's date
                    if (nextAvailableStartDate < DateTime.Today)
                    {
                        upcomingAvailableDates.Add((nextAvailableStartDate, DateTime.Today));
                    }

                    // Filter out past dates and take the next 5 available date ranges
                    upcomingAvailableDates = upcomingAvailableDates
                        .Where(d => d.StartDate >= DateTime.Today)
                        .Take(10) // Adjust as needed, fetch a reasonable number of upcoming dates
                        .ToList();

                    // Pass upcoming available dates to the notification view
                    ViewBag.AvailableDates = upcomingAvailableDates;
                    ViewBag.LastDate = LastNotAvailableDate;
                    var curViemModel = DetailsModel(viewModel);
                    return View("DetailsAndCreateReservation", curViemModel);
                }

                viewModel.Reservation.RoomID = viewModel.Room.RoomID;
                viewModel.Reservation.AppUserID = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.Add(viewModel.Reservation);
                _context.SaveChanges();

                return RedirectToAction("Index", "Reservations");
            }

            var newViemModel = DetailsModel(viewModel);

            return View("DetailsAndCreateReservation", newViemModel); // Ensure correct view name
        }

        public RoomReservationViewModel DetailsModel(RoomReservationViewModel viewModel)
        {
            var room = _context.Room
                    .Include(r => r.RoomContents)
                    .Include(r => r.Reservations)
                    .FirstOrDefault(r => r.RoomID == viewModel.Room.RoomID);

            if (room == null)
            {
                return null;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users
                .Include(u => u.Reservations)
                .FirstOrDefault(u => u.Id == userId);

            var newViewModel = new RoomReservationViewModel
            {
                Room = room,
                AppUser = user ?? new AppUser()
            };

            return newViewModel;
        }

        // GET: Room/Create
        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Room/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Create(Room room, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                if (Photo != null && Photo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Photo.CopyToAsync(memoryStream);
                        room.Photo = memoryStream.ToArray();
                    }
                }

                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }


        // GET: Rooms/Edit/5
        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var room = _context.Room
                .Include(r => r.RoomContents)
                .FirstOrDefault(r => r.RoomID == id);

            if (room == null)
            {
                return NotFound();
            }

            return View("Edit", room); // Ensure correct view name
        }

        // POST: Room/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int id, Room room, IFormFile? newPhoto)
        {
            if (id != room.RoomID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (newPhoto != null && newPhoto.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await newPhoto.CopyToAsync(memoryStream);
                            room.Photo = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        // Retain the existing photo if a new one is not uploaded
                        room.Photo = (await _context.Room.AsNoTracking().FirstOrDefaultAsync(r => r.RoomID == id))?.Photo;
                    }

                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.RoomID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.RoomID == id);
        }

        // GET: Room/Delete/5
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .Include(r => r.RoomContents)
                .FirstOrDefaultAsync(m => m.RoomID == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = UserRoles.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room.FindAsync(id);
            if (room != null)
            {
                var reservations = _context.Reservations.Where(r => r.RoomID == id);
                _context.Reservations.RemoveRange(reservations);

                _context.Room.Remove(room);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
