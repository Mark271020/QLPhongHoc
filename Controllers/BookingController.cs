using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLPhongHoc.Models;
using QLPhongHoc.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace QLPhongHoc.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly FirebaseService _firebaseService;

        public BookingController(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue("UserId");
            var bookings = await _firebaseService.GetBookingsByUserAsync(userId);
            
            foreach (var booking in bookings)
            {
                var room = await _firebaseService.GetPhongHocByIdAsync(booking.RoomId);
                booking.RoomId = room?.TenPhong ?? booking.RoomId;
            }
            
            return View(bookings);
        }

        public async Task<IActionResult> Create(string roomId = null)
        {
            ViewBag.Rooms = await _firebaseService.GetAllPhongHocAsync();
            ViewBag.RoomId = roomId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Validate thời gian
                if (booking.StartTime >= booking.EndTime)
                {
                    TempData["Error"] = "Thời gian kết thúc phải sau thời gian bắt đầu!";
                    ViewBag.Rooms = await _firebaseService.GetAllPhongHocAsync();
                    return View(booking);
                }
                
                if (booking.StartTime < DateTime.Now)
                {
                    TempData["Error"] = "Không thể đặt phòng trong quá khứ!";
                    ViewBag.Rooms = await _firebaseService.GetAllPhongHocAsync();
                    return View(booking);
                }
                
                // Kiểm tra phòng trống
                var isAvailable = await _firebaseService.CheckRoomAvailabilityAsync(
                    booking.RoomId, booking.StartTime, booking.EndTime);
                
                if (!isAvailable)
                {
                    var room = await _firebaseService.GetPhongHocByIdAsync(booking.RoomId);
                    TempData["Error"] = $"Phòng {room?.TenPhong} đã được đặt trong khoảng thời gian này! Vui lòng chọn thời gian khác.";
                    ViewBag.Rooms = await _firebaseService.GetAllPhongHocAsync();
                    return View(booking);
                }

                booking.UserId = User.FindFirstValue("UserId");
                booking.Status = "Pending";
                await _firebaseService.CreateBookingAsync(booking);
                
                var roomInfo = await _firebaseService.GetPhongHocByIdAsync(booking.RoomId);
                TempData["Success"] = $"Đặt phòng {roomInfo?.TenPhong} thành công! Thời gian: {booking.StartTime:dd/MM/yyyy HH:mm} - {booking.EndTime:HH:mm}. Vui lòng chờ admin duyệt.";
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Rooms = await _firebaseService.GetAllPhongHocAsync();
            return View(booking);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage()
        {
            var bookings = await _firebaseService.GetAllBookingsAsync();
            
            foreach (var booking in bookings)
            {
                var room = await _firebaseService.GetPhongHocByIdAsync(booking.RoomId);
                booking.RoomId = room?.TenPhong ?? booking.RoomId;
                
                var user = await _firebaseService.GetUserByIdAsync(booking.UserId);
                booking.UserId = user?.FullName ?? user?.Username ?? booking.UserId;
            }
            
            return View(bookings);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Approve(string id)
        {
            var booking = await _firebaseService.GetBookingByIdAsync(id);
            if (booking != null)
            {
                await _firebaseService.UpdateBookingStatusAsync(id, "Approved");
                var room = await _firebaseService.GetPhongHocByIdAsync(booking.RoomId);
                var user = await _firebaseService.GetUserByIdAsync(booking.UserId);
                TempData["Success"] = $"Đã duyệt đặt phòng của {user?.FullName} - Phòng {room?.TenPhong} vào lúc {booking.StartTime:dd/MM/yyyy HH:mm}";
            }
            return RedirectToAction(nameof(Manage));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Reject(string id)
        {
            var booking = await _firebaseService.GetBookingByIdAsync(id);
            if (booking != null)
            {
                await _firebaseService.UpdateBookingStatusAsync(id, "Rejected");
                var room = await _firebaseService.GetPhongHocByIdAsync(booking.RoomId);
                var user = await _firebaseService.GetUserByIdAsync(booking.UserId);
                TempData["Error"] = $"Đã từ chối đặt phòng của {user?.FullName} - Phòng {room?.TenPhong}. Lý do: Phòng đã có lịch khác hoặc không phù hợp.";
            }
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(string id)
        {
            var userId = User.FindFirstValue("UserId");
            var booking = await _firebaseService.GetBookingByIdAsync(id);
            
            if (booking != null && booking.UserId == userId && booking.Status == "Pending")
            {
                await _firebaseService.UpdateBookingStatusAsync(id, "Cancelled");
                var room = await _firebaseService.GetPhongHocByIdAsync(booking.RoomId);
                TempData["Success"] = $"Đã hủy đặt phòng {room?.TenPhong} thành công!";
            }
            else
            {
                TempData["Error"] = "Không thể hủy đặt phòng này!";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}