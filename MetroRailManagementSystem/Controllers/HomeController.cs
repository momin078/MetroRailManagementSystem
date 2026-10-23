using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MetroRailManagementSystem.Data;
using MetroRailManagementSystem.ViewModels;
using System.Diagnostics;
using MetroRailManagementSystem.Models;

namespace MetroRailManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            AppDbContext db,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Admin only: show counts
            if (User.IsInRole("Admin"))
            {
                var vm = new DashboardVM
                {
                    TotalStations = await _db.Stations.CountAsync(),
                    TotalTrains = await _db.Trains.CountAsync(),
                    TotalTickets = await _db.Tickets.CountAsync(),
                    AvailableSeats = await _db.Trains.SumAsync(t => t.AvailableSeats),
                    TotalUsers = await _userManager.Users.CountAsync()
                };

                return View(vm);
            }

            // User dashboard (no counts needed)
            return View(new DashboardVM());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
