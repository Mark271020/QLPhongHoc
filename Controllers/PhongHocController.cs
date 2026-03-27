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

        // Tất cả người dùng đều có thể xem danh sách phòng
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var list = await _firebaseService.GetAllPhongHocAsync();
            
            if (!string.IsNullOrEmpty(searchString))
            {
                list = await _firebaseService.SearchPhongHocAsync(searchString);
            }
            
            return View(list);
        }

        // Tất cả người dùng đều có thể xem chi tiết phòng
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            
            var item = await _firebaseService.GetPhongHocByIdAsync(id);
            if (item == null) return NotFound();
            
            return View(item);
        }

        // CHỈ ADMIN mới được tạo phòng
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenPhong,ViTri,SucChua,TrangThietBi,IsAvailable,MoTa")] PhongHoc phongHoc)
        {
            if (ModelState.IsValid)
            {
                await _firebaseService.AddPhongHocAsync(phongHoc);
                TempData["Success"] = "Thêm phòng học thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(phongHoc);
        }

        // CHỈ ADMIN mới được sửa phòng
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            
            var item = await _firebaseService.GetPhongHocByIdAsync(id);
            if (item == null) return NotFound();
            
            return View(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,TenPhong,ViTri,SucChua,TrangThietBi,IsAvailable,MoTa,CreatedAt")] PhongHoc phongHoc)
        {
            if (id != phongHoc.Id) return NotFound();
            
            if (ModelState.IsValid)
            {
                await _firebaseService.UpdatePhongHocAsync(id, phongHoc);
                TempData["Success"] = "Cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(phongHoc);
        }

        // CHỈ ADMIN mới được xóa phòng
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            
            var item = await _firebaseService.GetPhongHocByIdAsync(id);
            if (item == null) return NotFound();
            
            return View(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _firebaseService.DeletePhongHocAsync(id);
            TempData["Success"] = "Xóa thành công!";
            return RedirectToAction(nameof(Index));
        }

        // CHỈ ADMIN mới xem thống kê
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Statistics()
        {
            var stats = await _firebaseService.GetStatisticsAsync();
            return View(stats);
        }
    }
}