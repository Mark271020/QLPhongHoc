using Microsoft.AspNetCore.Mvc;
using QLPhongHoc.Services;
using QLPhongHoc.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace QLPhongHoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly FirebaseService _firebaseService;

        public HomeController(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var stats = await _firebaseService.GetStatisticsAsync();
                return View(stats);
            }
            catch (System.Exception ex)
            {
                // Nếu có lỗi, trả về dữ liệu mặc định
                var defaultStats = new Dictionary<string, object>
                {
                    { "TotalRooms", 0 },
                    { "AvailableRooms", 0 },
                    { "UnavailableRooms", 0 },
                    { "TotalCapacity", 0 },
                    { "AverageCapacity", 0 }
                };
                return View(defaultStats);
            }
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