using MetroRailManagementSystem.Data;
using MetroRailManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MetroRailManagementSystem.Controllers
{
    [Authorize]
    public class TrainsController : Controller
    {
        private readonly AppDbContext _db;

        public TrainsController(AppDbContext db)
        {
            _db = db;
        }

        // Admin + User can view
        public async Task<IActionResult> Index()
        {
            var trains = await _db.Trains.OrderBy(t => t.Name).ToListAsync();
            return View(trains);
        }

        // Admin only
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        // Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Train train)
        {
            if (!ModelState.IsValid) return View(train);

            if (train.AvailableSeats > train.TotalSeats)
            {
                ModelState.AddModelError(nameof(train.AvailableSeats), "Available seats cannot be greater than total seats.");
                return View(train);
            }

            _db.Trains.Add(train);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var train = await _db.Trains.FindAsync(id);
            if (train == null) return NotFound();
            return View(train);
        }

        // Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Train train)
        {
            if (id != train.Id) return BadRequest();

            if (!ModelState.IsValid) return View(train);

            if (train.AvailableSeats > train.TotalSeats)
            {
                ModelState.AddModelError(nameof(train.AvailableSeats), "Available seats cannot be greater than total seats.");
                return View(train);
            }

            _db.Update(train);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var train = await _db.Trains.FindAsync(id);
            if (train == null) return NotFound();
            return View(train);
        }

        // Admin only
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var train = await _db.Trains.FindAsync(id);
            if (train == null) return NotFound();

            _db.Trains.Remove(train);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
