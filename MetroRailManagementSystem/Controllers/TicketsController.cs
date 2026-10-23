using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MetroRailManagementSystem.Data;
using MetroRailManagementSystem.Models;
using MetroRailManagementSystem.ViewModels;

namespace MetroRailManagementSystem.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        private const decimal FarePerKm = 5m;

        public TicketsController(AppDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);

            IQueryable<Ticket> query = _db.Tickets.OrderByDescending(t => t.Id);

            // Admin sees all. User sees only own.
            if (!User.IsInRole("Admin"))
                query = query.Where(t => t.UserId == currentUserId);

            var tickets = await query.ToListAsync();

            var stationMap = await _db.Stations.ToDictionaryAsync(s => s.Id, s => s.Name);
            var trainMap = await _db.Trains.ToDictionaryAsync(t => t.Id, t => t.Name);

            ViewBag.StationMap = stationMap;
            ViewBag.TrainMap = trainMap;

            return View(tickets);
        }

        // Both Admin + User can book
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View(new TicketCreateVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(vm);
            }

            if (vm.FromStationId == vm.ToStationId)
            {
                ModelState.AddModelError(nameof(vm.ToStationId), "From and To station cannot be the same.");
                await LoadDropdowns();
                return View(vm);
            }

            // ✅ Payment number required (bkash/rocket/nagad/bank)
            if (string.IsNullOrWhiteSpace(vm.PaymentNumber))
            {
                ModelState.AddModelError(nameof(vm.PaymentNumber), "Payment number / account is required.");
                await LoadDropdowns();
                return View(vm);
            }

            // ✅ Stations (for distance)
            var fromStation = await _db.Stations.FirstOrDefaultAsync(s => s.Id == vm.FromStationId && s.IsActive);
            var toStation = await _db.Stations.FirstOrDefaultAsync(s => s.Id == vm.ToStationId && s.IsActive);

            if (fromStation == null || toStation == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid station selection.");
                await LoadDropdowns();
                return View(vm);
            }

            var distanceKm = Math.Abs(toStation.DistanceKm - fromStation.DistanceKm);
            if (distanceKm <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid distance between stations. Set Station DistanceKm properly.");
                await LoadDropdowns();
                return View(vm);
            }

            // ✅ Train seat check
            var train = await _db.Trains.FirstOrDefaultAsync(t => t.Id == vm.TrainId && t.IsActive);
            if (train == null)
            {
                ModelState.AddModelError(nameof(vm.TrainId), "Invalid train selected.");
                await LoadDropdowns();
                return View(vm);
            }

            if (train.AvailableSeats < vm.Quantity)
            {
                ModelState.AddModelError(nameof(vm.Quantity), $"Not enough seats. Available: {train.AvailableSeats}");
                await LoadDropdowns();
                return View(vm);
            }

            // ✅ Fare calculation: 5 TK per KM
            var totalFare = (decimal)distanceKm * FarePerKm * vm.Quantity;

            // reduce seats
            train.AvailableSeats -= vm.Quantity;

            var ticket = new Ticket
            {
                PassengerName = vm.PassengerName,
                FromStationId = vm.FromStationId,
                ToStationId = vm.ToStationId,
                TrainId = vm.TrainId,
                Quantity = vm.Quantity,

                DistanceKm = distanceKm,
                FarePerKm = FarePerKm,
                TotalFare = totalFare,

                PaymentMethod = vm.PaymentMethod,
                PaymentNumber = vm.PaymentNumber,
                PaymentStatus = "Paid",

                UserId = _userManager.GetUserId(User),

                IsCancelled = false,
                CancelledAt = null,
                CancelledByUserId = null
            };

            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ✅ PDF Metro Card page (prints using browser print)
        public async Task<IActionResult> Pdf(int id)
        {
            var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);

            // User can access only their own ticket, Admin can access all
            if (!User.IsInRole("Admin") && ticket.UserId != currentUserId)
                return Forbid();

            var fromStation = await _db.Stations.FirstOrDefaultAsync(s => s.Id == ticket.FromStationId);
            var toStation = await _db.Stations.FirstOrDefaultAsync(s => s.Id == ticket.ToStationId);
            var train = await _db.Trains.FirstOrDefaultAsync(tr => tr.Id == ticket.TrainId);

            ViewBag.FromStation = fromStation?.Name ?? "N/A";
            ViewBag.ToStation = toStation?.Name ?? "N/A";
            ViewBag.TrainName = train?.Name ?? "N/A";

            return View(ticket); // Views/Tickets/Pdf.cshtml
        }

        // Cancel ticket: Admin can cancel any, user can cancel only their own
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);

            if (!User.IsInRole("Admin") && ticket.UserId != currentUserId)
                return Forbid();

            if (ticket.IsCancelled)
                return RedirectToAction(nameof(Index));

            var train = await _db.Trains.FirstOrDefaultAsync(t => t.Id == ticket.TrainId);
            if (train != null)
            {
                train.AvailableSeats += ticket.Quantity;
                if (train.AvailableSeats > train.TotalSeats)
                    train.AvailableSeats = train.TotalSeats;
            }

            ticket.IsCancelled = true;
            ticket.CancelledAt = DateTime.Now;
            ticket.CancelledByUserId = currentUserId;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            var stations = await _db.Stations
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();

            var trains = await _db.Trains
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();

            ViewBag.Trains = new SelectList(trains, "Id", "Name");
            ViewBag.StationsRaw = stations;
        }
    }
}
