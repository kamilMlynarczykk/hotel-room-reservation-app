using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelReservationApp.Data;
using HotelReservationApp.Models;
using HotelReservationApp.Models.ViewModels;
using System.Globalization;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HotelReservationApp.Controllers
{
    public class ReservationHistoriesController : Controller
    {
        private readonly HotelReservationAppContext _context;

        public ReservationHistoriesController(HotelReservationAppContext context)
        {
            _context = context;
        }

        // GET: ReservationHistories
        public async Task<IActionResult> Index()
        {
            return View(await _context.ReservationHistories.ToListAsync());
        }

        // GET: ReservationHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationHistory = await _context.ReservationHistories
                .FirstOrDefaultAsync(m => m.ReservationHistoryID == id);
            if (reservationHistory == null)
            {
                return NotFound();
            }

            return View(reservationHistory);
        }

        // GET: ReservationHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationHistory = await _context.ReservationHistories
                .FirstOrDefaultAsync(m => m.ReservationHistoryID == id);
            if (reservationHistory == null)
            {
                return NotFound();
            }

            return View(reservationHistory);
        }

        // POST: ReservationHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservationHistory = await _context.ReservationHistories.FindAsync(id);
            if (reservationHistory != null)
            {
                _context.ReservationHistories.Remove(reservationHistory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationHistoryExists(int id)
        {
            return _context.ReservationHistories.Any(e => e.ReservationHistoryID == id);
        }

        // Background task to move expired reservations to ReservationHistory
        public async Task MoveExpiredReservationsToHistory()
        {
            var expiredReservations = await _context.Reservations
                .Where(r => r.EndDate < DateTime.Today && r.Status != StatusPhases.Archived)
                .ToListAsync();

            foreach (var reservation in expiredReservations)
            {
                var reservationHistory = new ReservationHistory
                {
                    StartDate = reservation.StartDate,
                    EndDate = reservation.EndDate,
                    AddedDate = reservation.AddedDate,
                    Status = StatusPhases.Archived,
                    ReservationID = reservation.ReservationID,
                    EmailAddress = reservation.AppUser.Email,
                    RoomNumber = reservation.Room.RoomNumber,
                };

                _context.ReservationHistories.Add(reservationHistory);
                reservation.Status = StatusPhases.Archived;
                _context.Reservations.Remove(reservation); 
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IActionResult> RoomReservationStats(int? selectedYear, int? selectedMonth, string selectedRoomNumber)
        {
            var viewModel = new RoomReservationStatsViewModel();

            // Get distinct years from ReservationHistory
            viewModel.Years = await _context.ReservationHistories
                .Select(r => r.StartDate.Year)
                .Distinct()
                .ToListAsync();

            // Set default values for year and month if not provided
            viewModel.SelectedYear = selectedYear ?? 0; // means all months
            viewModel.SelectedMonth = selectedMonth ?? 0;  // 0 means all months
            viewModel.SelectedRoomNumber = selectedRoomNumber;

            // Populate dropdowns
            viewModel.Months = Enumerable.Range(1, 12)
                .Select(monthNumber => new SelectListItem
                {
                    Value = monthNumber.ToString(),
                    Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber)
                })
                .ToList();

            viewModel.RoomNumbers = await _context.Room
                .Select(r => r.RoomNumber)
                .ToListAsync();

            // Fetch statistics based on selected filters
            if (viewModel.SelectedMonth != 0 && viewModel.SelectedYear == 0)
            {
                viewModel.ReservationStats = await GetStatisticsForMonthAcrossYears(viewModel.SelectedMonth, viewModel.SelectedRoomNumber);
            }
            else if (selectedMonth.HasValue && selectedMonth.Value != 0 && !string.IsNullOrWhiteSpace(selectedRoomNumber))
            {
                viewModel.ReservationStats = await GetStatisticsForRoomInMonth(viewModel.SelectedYear, selectedMonth.Value, selectedRoomNumber);
            }
            else if (selectedMonth.HasValue && selectedMonth.Value != 0)
            {
                viewModel.ReservationStats = await GetStatisticsForAllWeeks(viewModel.SelectedYear, selectedMonth.Value, selectedRoomNumber);
            }
            else if (!selectedMonth.HasValue && viewModel.SelectedYear != 0)
            {
                viewModel.ReservationStats = await GetStatisticsForAllMonths(viewModel.SelectedYear, selectedRoomNumber);
            }
            else
            {
                viewModel.ReservationStats = await GetStatisticsForAllYears(selectedRoomNumber);
            }

            // Assign the ViewBag to show data in the view
            ViewBag.ReservationStats = viewModel.ReservationStats;

            return View(viewModel);
        }

        // Helper method to get statistics for all years
        private async Task<List<ChartDataViewModel>> GetStatisticsForAllYears(string? roomNumber)
        {
            List<ReservationHistory> reservations;

            if (roomNumber == null)
            {
                reservations = await _context.ReservationHistories
                    .ToListAsync();
            }
            else
            {
                reservations = await _context.ReservationHistories
                    .Where(r => r.RoomNumber == roomNumber)
                    .ToListAsync();
            }

            var statistics = reservations
                .GroupBy(r => new { Year = r.StartDate.Year })
                .Select(g => new ChartDataViewModel
                {
                    Label = g.Key.Year.ToString(),
                    Value = g.Sum(r => (r.EndDate - r.StartDate).Days + 1)
                })
                .OrderBy(g => g.Label)
                .ToList();

            return statistics;
        }

        // Helper method to get statistics for all months in a year
        private async Task<List<ChartDataViewModel>> GetStatisticsForAllMonths(int year, string? roomNumber)
        {
            List<ReservationHistory> reservations;

            if (roomNumber == null)
            {
                reservations = await _context.ReservationHistories
                    .Where(r => r.StartDate.Year == year)
                    .ToListAsync();
            }
            else
            {
                reservations = await _context.ReservationHistories
                    .Where(r => r.StartDate.Year == year && r.RoomNumber == roomNumber)
                    .ToListAsync();
            }

            var statistics = reservations
                .GroupBy(r => new { Month = r.StartDate.Month })
                .Select(g => new ChartDataViewModel
                {
                    Label = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    Value = g.Sum(r => (r.EndDate - r.StartDate).Days + 1) // Total days reserved in the month
                })
                .OrderBy(g => DateTime.ParseExact(g.Label, "MMMM", CultureInfo.CurrentCulture).Month)
                .ToList();

            return statistics;
        }

        // Helper method to get statistics for all weeks in a month
        private async Task<List<ChartDataViewModel>> GetStatisticsForAllWeeks(int year, int month, string? roomNumber)
        {
            List<ReservationHistory> reservations;

            if (roomNumber == null)
            {
                reservations = await _context.ReservationHistories
                    .Where(r => r.StartDate.Year == year && r.StartDate.Month == month)
                    .ToListAsync();
            }
            else
            {
                reservations = await _context.ReservationHistories
                    .Where(r => r.StartDate.Year == year && r.StartDate.Month == month && r.RoomNumber == roomNumber)
                    .ToListAsync();
            }

            var statistics = reservations
                .GroupBy(r => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(r.StartDate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday))
                .Select(g => new ChartDataViewModel
                {
                    Label = $"Week {g.Key}",
                    Value = g.Sum(r => (r.EndDate - r.StartDate).Days + 1) // Total days reserved in the week
                })
                .OrderBy(g => g.Label)
                .ToList();  // Convert to List<ChartDataViewModel> here

            return statistics;
        }

        // Helper method to get statistics for a specific room in a month
        private async Task<List<ChartDataViewModel>> GetStatisticsForRoomInMonth(int? year, int month, string roomNumber)
        {
            List<ReservationHistory> reservations;
            if (year != 0)
            {
                reservations = await _context.ReservationHistories
                .Where(r => r.StartDate.Year == year && r.StartDate.Month == month && r.RoomNumber == roomNumber)
                .ToListAsync();
            }
            else
            {
                reservations = await _context.ReservationHistories
                    .Where(r => r.StartDate.Month == month && r.RoomNumber == roomNumber)
                    .ToListAsync();
            }


            var statistics = reservations
                .GroupBy(r => new { Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(r.StartDate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) })
                .Select(g => new ChartDataViewModel
                {
                    Label = $"Week {g.Key.Week}",
                    Value = g.Sum(r => (r.EndDate - r.StartDate).Days + 1) // Total days reserved in the week
                })
                .OrderBy(g => g.Label)
                .ToList();

            return statistics;
        }

        private async Task<List<ChartDataViewModel>> GetStatisticsForMonthAcrossYears(int month, string? roomNumber)
        {
            List<ReservationHistory> reservations;

            if (roomNumber == null)
            {
                reservations = await _context.ReservationHistories
                    .Where(r => r.StartDate.Month == month)
                    .ToListAsync();
            }
            else
            {
                reservations = await _context.ReservationHistories
                    .Where(r => r.StartDate.Month == month && r.RoomNumber == roomNumber)
                    .ToListAsync();
            }

            var statistics = reservations
                .GroupBy(r => r.StartDate.Year)
                .Select(g => new ChartDataViewModel
                {
                    Label = g.Key.ToString(),
                    Value = g.Sum(r => (r.EndDate - r.StartDate).Days + 1) // Total days reserved in the month for each year
                })
                .OrderBy(g => g.Label)
                .ToList();

            return statistics;
        }
    }
}
