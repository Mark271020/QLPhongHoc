using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLPhongHoc.Models;
using QLPhongHoc.Services;
using System.Threading.Tasks;

namespace QLPhongHoc.Controllers
{
    public class PhongHocController : Controller
    {
        private readonly FirebaseService _firebaseService;

        public PhongHocController(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var phongHocs = await _firebaseService.GetAllPhongHocAsync();
            
            if (!string.IsNullOrEmpty(searchString))
            {
                phongHocs = await _firebaseService.SearchPhongHocAsync(searchString);
            }
            
            return View(phongHocs);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            
            var phongHoc = await _firebaseService.GetPhongHocByIdAsync(id);
            if (phongHoc == null) return NotFound();
            
            return View(phongHoc);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            Console.WriteLine("=== VÀO TRANG CREATE ===");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenPhong,ViTri,SucChua,TrangThietBi,IsAvailable,MoTa")] PhongHoc phongHoc)
        {
            Console.WriteLine("=== BẮT ĐẦU THÊM PHÒNG ===");
            Console.WriteLine($"Tên phòng: {phongHoc.TenPhong}");
            Console.WriteLine($"Vị trí: {phongHoc.ViTri}");
            Console.WriteLine($"Sức chứa: {phongHoc.SucChua}");
            Console.WriteLine($"IsAvailable: {phongHoc.IsAvailable}");
            
            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState hợp lệ");
                try
                {
                    Console.WriteLine("Đang gọi AddPhongHocAsync...");
                    string newId = await _firebaseService.AddPhongHocAsync(phongHoc);
                    Console.WriteLine($"✅ THÀNH CÔNG! ID mới: {newId}");
                    TempData["Success"] = $"✅ Đã thêm phòng '{phongHoc.TenPhong}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ LỖI: {ex.Message}");
                    Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                    TempData["Error"] = $"❌ Lỗi: {ex.Message}";
                    return View(phongHoc);
                }
            }
            else
            {
                Console.WriteLine("ModelState KHÔNG hợp lệ:");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"  - {key}: {error.ErrorMessage}");
                    }
                }
            }
            
            return View(phongHoc);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            
            var phongHoc = await _firebaseService.GetPhongHocByIdAsync(id);
            if (phongHoc == null) return NotFound();
            
            return View(phongHoc);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,TenPhong,ViTri,SucChua,TrangThietBi,IsAvailable,MoTa,CreatedAt")] PhongHoc phongHoc)
        {
            if (id != phongHoc.Id) return NotFound();
            
            if (ModelState.IsValid)
            {
                try
                {
                    await _firebaseService.UpdatePhongHocAsync(id, phongHoc);
                    TempData["Success"] = $"✅ Đã cập nhật phòng '{phongHoc.TenPhong}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"❌ Lỗi: {ex.Message}";
                    return View(phongHoc);
                }
            }
            
            return View(phongHoc);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            
            var phongHoc = await _firebaseService.GetPhongHocByIdAsync(id);
            if (phongHoc == null) return NotFound();
            
            return View(phongHoc);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var phongHoc = await _firebaseService.GetPhongHocByIdAsync(id);
                await _firebaseService.DeletePhongHocAsync(id);
                TempData["Success"] = $"✅ Đã xóa phòng '{phongHoc?.TenPhong}' thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Statistics()
        {
            var stats = await _firebaseService.GetStatisticsAsync();
            return View(stats);
        }
    }
}