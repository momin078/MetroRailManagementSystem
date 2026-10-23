using MetroRailManagementSystem.Data;
using MetroRailManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MetroRailManagementSystem.Controllers
{
    [Authorize]
    public class StationsController : Controller
    {
        private readonly AppDbContext _db;

        public StationsController(AppDbContext db)
        {
            _db = db;
        }

        // Admin + User can view
        public async Task<IActionResult> Index()
        {
            var stations = await _db.Stations.OrderBy(s => s.Name).ToListAsync();
            return View(stations);
        }

        // Admin only
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        // Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Station station)
        {
            if (!ModelState.IsValid) return View(station);

            var exists = await _db.Stations.AnyAsync(s => s.Code == station.Code);
            if (exists)
            {
                ModelState.AddModelError("Code", "This station code already exists.");
                return View(station);
            }

            _db.Stations.Add(station);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var station = await _db.Stations.FindAsync(id);
            if (station == null) return NotFound();
            return View(station);
        }

        // Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Station station)
        {
            if (id != station.Id) return BadRequest();

            if (!ModelState.IsValid) return View(station);

            var exists = await _db.Stations.AnyAsync(s => s.Code == station.Code && s.Id != station.Id);
            if (exists)
            {
                ModelState.AddModelError("Code", "This station code already exists.");
                return View(station);
            }

            _db.Update(station);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var station = await _db.Stations.FindAsync(id);
            if (station == null) return NotFound();
            return View(station);
        }

        // Admin only
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var station = await _db.Stations.FindAsync(id);
            if (station == null) return NotFound();

            _db.Stations.Remove(station);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
