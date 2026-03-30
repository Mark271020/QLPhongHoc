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

        // GET: Danh sách phòng
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

        // GET: Chi tiết phòng
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Không tìm thấy ID phòng!";
                return RedirectToAction(nameof(Index));
            }
            
            var phongHoc = await _firebaseService.GetPhongHocByIdAsync(id);
            if (phongHoc == null)
            {
                TempData["Error"] = "Không tìm thấy phòng học!";
                return RedirectToAction(nameof(Index));
            }
            
            return View(phongHoc);
        }

        // GET: Tạo phòng
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tạo phòng
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhongHoc phongHoc)
        {
            ModelState.Remove("Id");
            
            if (ModelState.IsValid)
            {
                try
                {
                    await _firebaseService.AddPhongHocAsync(phongHoc);
                    TempData["Success"] = $"✅ Đã thêm phòng '{phongHoc.TenPhong}' thành công!";
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

        // GET: Sửa phòng
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Không tìm thấy ID phòng!";
                return RedirectToAction(nameof(Index));
            }
            
            var phongHoc = await _firebaseService.GetPhongHocByIdAsync(id);
            if (phongHoc == null)
            {
                TempData["Error"] = "Không tìm thấy phòng học!";
                return RedirectToAction(nameof(Index));
            }
            
            return View(phongHoc);
        }

        // POST: Sửa phòng
            [Authorize(Roles = "Admin")]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(string id, PhongHoc phongHoc)
{
    if (id != phongHoc.Id)
    {
        TempData["Error"] = "ID không khớp!";
        return RedirectToAction(nameof(Index));
    }
    
    // Bỏ qua validation của CreatedAt (vì nó đã có sẵn)
    ModelState.Remove("CreatedAt");
    
    if (ModelState.IsValid)
    {
        try
        {
            // Lấy phòng cũ để giữ nguyên CreatedAt nếu cần
            var existingRoom = await _firebaseService.GetPhongHocByIdAsync(id);
            if (existingRoom != null)
            {
                phongHoc.CreatedAt = existingRoom.CreatedAt;
            }
            
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

        // GET: Xóa phòng
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Không tìm thấy ID phòng!";
                return RedirectToAction(nameof(Index));
            }
            
            var phongHoc = await _firebaseService.GetPhongHocByIdAsync(id);
            if (phongHoc == null)
            {
                TempData["Error"] = "Không tìm thấy phòng học!";
                return RedirectToAction(nameof(Index));
            }
            
            return View(phongHoc);
        }

        // POST: Xóa phòng
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Không tìm thấy ID phòng!";
                return RedirectToAction(nameof(Index));
            }
            
            try
            {
                var phongHoc = await _firebaseService.GetPhongHocByIdAsync(id);
                if (phongHoc == null)
                {
                    TempData["Error"] = "Không tìm thấy phòng học!";
                    return RedirectToAction(nameof(Index));
                }
                
                await _firebaseService.DeletePhongHocAsync(id);
                TempData["Success"] = $"✅ Đã xóa phòng '{phongHoc.TenPhong}' thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Thống kê
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Statistics()
        {
            var stats = await _firebaseService.GetStatisticsAsync();
            return View(stats);
        }
    }
}