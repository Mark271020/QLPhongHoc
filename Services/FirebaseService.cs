using Google.Cloud.Firestore;
using QLPhongHoc.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QLPhongHoc.Services
{
    public class FirebaseService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string PhongHocCollection = "phonghoc";
        private const string UserCollection = "users";
        private const string BookingCollection = "bookings";

        public FirebaseService(IConfiguration configuration)
        {
            try
            {
                string path = configuration["Firebase:CredentialPath"];
                string projectId = configuration["Firebase:ProjectId"];
                
                if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(projectId))
                {
                    throw new Exception("Chưa cấu hình Firebase trong appsettings.json");
                }
                
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
                
                Console.WriteLine($"Đang tìm file tại: {fullPath}");
                Console.WriteLine("Các file trong thư mục hiện tại:");
                foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory()))
                {
                    Console.WriteLine($"  - {file}");
                }
                
                if (!File.Exists(fullPath))
                {
                    throw new Exception($"Không tìm thấy file JSON tại: {fullPath}");
                }
                
                string jsonContent = File.ReadAllText(fullPath);
                Console.WriteLine($"Đã đọc file JSON, độ dài: {jsonContent.Length} ký tự");
                
                _firestoreDb = new FirestoreDbBuilder
                {
                    ProjectId = projectId,
                    JsonCredentials = jsonContent
                }.Build();
                
                Console.WriteLine($"✅ Kết nối Firebase thành công! Project: {projectId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi kết nối Firebase: {ex.Message}");
                throw;
            }
        }

        // ==================== PHÒNG HỌC ====================

        public async Task<List<PhongHoc>> GetAllPhongHocAsync()
        {
            try
            {
                Query query = _firestoreDb.Collection(PhongHocCollection);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();
                
                var phongHocList = new List<PhongHoc>();
                foreach (var document in snapshot.Documents)
                {
                    var phongHoc = document.ConvertTo<PhongHoc>();
                    phongHoc.Id = document.Id;
                    phongHocList.Add(phongHoc);
                }
                
                return phongHocList.OrderBy(p => p.TenPhong).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách phòng: {ex.Message}");
            }
        }

        public async Task<PhongHoc> GetPhongHocByIdAsync(string id)
        {
            try
            {
                DocumentReference docRef = _firestoreDb.Collection(PhongHocCollection).Document(id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                
                if (snapshot.Exists)
                {
                    var phongHoc = snapshot.ConvertTo<PhongHoc>();
                    phongHoc.Id = snapshot.Id;
                    return phongHoc;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy chi tiết phòng: {ex.Message}");
            }
        }

        public async Task<string> AddPhongHocAsync(PhongHoc phongHoc)
        {
            try
            {
                phongHoc.CreatedAt = DateTime.UtcNow;
                DocumentReference docRef = _firestoreDb.Collection(PhongHocCollection).Document();
                await docRef.SetAsync(phongHoc);
                return docRef.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm phòng: {ex.Message}");
            }
        }

        public async Task<bool> UpdatePhongHocAsync(string id, PhongHoc phongHoc)
        {
            try
            {
                DocumentReference docRef = _firestoreDb.Collection(PhongHocCollection).Document(id);
                await docRef.SetAsync(phongHoc, SetOptions.MergeAll);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật phòng: {ex.Message}");
            }
        }

        public async Task<bool> DeletePhongHocAsync(string id)
        {
            try
            {
                DocumentReference docRef = _firestoreDb.Collection(PhongHocCollection).Document(id);
                await docRef.DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa phòng: {ex.Message}");
            }
        }

        public async Task<List<PhongHoc>> SearchPhongHocAsync(string keyword)
        {
            var allRooms = await GetAllPhongHocAsync();
            if (string.IsNullOrEmpty(keyword)) return allRooms;
            
            keyword = keyword.ToLower();
            return allRooms.Where(p => 
                p.TenPhong.ToLower().Contains(keyword) ||
                p.ViTri.ToLower().Contains(keyword) ||
                (p.MoTa != null && p.MoTa.ToLower().Contains(keyword))
            ).ToList();
        }

        public async Task<Dictionary<string, object>> GetStatisticsAsync()
        {
            var allRooms = await GetAllPhongHocAsync();
            
            return new Dictionary<string, object>
            {
                { "TotalRooms", allRooms.Count },
                { "AvailableRooms", allRooms.Count(r => r.IsAvailable) },
                { "UnavailableRooms", allRooms.Count(r => !r.IsAvailable) },
                { "TotalCapacity", allRooms.Sum(r => r.SucChua) },
                { "AverageCapacity", allRooms.Count > 0 ? Math.Round(allRooms.Average(r => r.SucChua), 2) : 0 }
            };
        }

        // ==================== NGƯỜI DÙNG ====================

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var query = _firestoreDb.Collection(UserCollection).WhereEqualTo("Username", username);
            var snapshot = await query.GetSnapshotAsync();
            
            if (snapshot.Documents.Count > 0)
            {
                var user = snapshot.Documents[0].ConvertTo<User>();
                user.Id = snapshot.Documents[0].Id;
                return user;
            }
            return null;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var query = _firestoreDb.Collection(UserCollection).WhereEqualTo("Email", email);
            var snapshot = await query.GetSnapshotAsync();
            
            if (snapshot.Documents.Count > 0)
            {
                var user = snapshot.Documents[0].ConvertTo<User>();
                user.Id = snapshot.Documents[0].Id;
                return user;
            }
            return null;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var docRef = _firestoreDb.Collection(UserCollection).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();
            
            if (snapshot.Exists)
            {
                var user = snapshot.ConvertTo<User>();
                user.Id = snapshot.Id;
                return user;
            }
            return null;
        }

        public async Task<string> CreateUserAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            var docRef = _firestoreDb.Collection(UserCollection).Document();
            await docRef.SetAsync(user);
            return docRef.Id;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null) return false;
            
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public async Task<bool> UpdateUserAsync(string id, User user)
        {
            var docRef = _firestoreDb.Collection(UserCollection).Document(id);
            await docRef.SetAsync(user, SetOptions.MergeAll);
            return true;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var snapshot = await _firestoreDb.Collection(UserCollection).GetSnapshotAsync();
            var users = new List<User>();
            foreach (var doc in snapshot.Documents)
            {
                var user = doc.ConvertTo<User>();
                user.Id = doc.Id;
                users.Add(user);
            }
            return users.OrderBy(u => u.Username).ToList();
        }

        // ==================== ĐẶT PHÒNG ====================

        public async Task<string> CreateBookingAsync(Booking booking)
        {
            booking.CreatedAt = DateTime.UtcNow;
            var docRef = _firestoreDb.Collection(BookingCollection).Document();
            await docRef.SetAsync(booking);
            return docRef.Id;
        }

        public async Task<List<Booking>> GetBookingsByUserAsync(string userId)
        {
            var query = _firestoreDb.Collection(BookingCollection).WhereEqualTo("UserId", userId);
            var snapshot = await query.GetSnapshotAsync();
            
            var bookings = new List<Booking>();
            foreach (var doc in snapshot.Documents)
            {
                var booking = doc.ConvertTo<Booking>();
                booking.Id = doc.Id;
                bookings.Add(booking);
            }
            return bookings.OrderByDescending(b => b.CreatedAt).ToList();
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            var snapshot = await _firestoreDb.Collection(BookingCollection).GetSnapshotAsync();
            var bookings = new List<Booking>();
            foreach (var doc in snapshot.Documents)
            {
                var booking = doc.ConvertTo<Booking>();
                booking.Id = doc.Id;
                bookings.Add(booking);
            }
            return bookings.OrderByDescending(b => b.CreatedAt).ToList();
        }

        public async Task<Booking> GetBookingByIdAsync(string id)
        {
            var docRef = _firestoreDb.Collection(BookingCollection).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();
            
            if (snapshot.Exists)
            {
                var booking = snapshot.ConvertTo<Booking>();
                booking.Id = snapshot.Id;
                return booking;
            }
            return null;
        }

        public async Task<bool> UpdateBookingStatusAsync(string bookingId, string status)
        {
            var docRef = _firestoreDb.Collection(BookingCollection).Document(bookingId);
            await docRef.UpdateAsync("Status", status);
            return true;
        }

        public async Task<bool> UpdateBookingAsync(string id, Booking booking)
        {
            var docRef = _firestoreDb.Collection(BookingCollection).Document(id);
            await docRef.SetAsync(booking, SetOptions.MergeAll);
            return true;
        }

        public async Task<bool> DeleteBookingAsync(string id)
        {
            var docRef = _firestoreDb.Collection(BookingCollection).Document(id);
            await docRef.DeleteAsync();
            return true;
        }

        public async Task<bool> CheckRoomAvailabilityAsync(string roomId, DateTime startTime, DateTime endTime)
        {
            var query = _firestoreDb.Collection(BookingCollection)
                .WhereEqualTo("RoomId", roomId)
                .WhereEqualTo("Status", "Approved");
            
            var snapshot = await query.GetSnapshotAsync();
            
            foreach (var doc in snapshot.Documents)
            {
                var booking = doc.ConvertTo<Booking>();
                if (startTime < booking.EndTime && endTime > booking.StartTime)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<List<Booking>> GetBookingsByRoomAsync(string roomId)
        {
            var query = _firestoreDb.Collection(BookingCollection).WhereEqualTo("RoomId", roomId);
            var snapshot = await query.GetSnapshotAsync();
            
            var bookings = new List<Booking>();
            foreach (var doc in snapshot.Documents)
            {
                var booking = doc.ConvertTo<Booking>();
                booking.Id = doc.Id;
                bookings.Add(booking);
            }
            return bookings.OrderByDescending(b => b.BookingDate).ToList();
        }

        public async Task<Dictionary<string, object>> GetBookingStatisticsAsync()
        {
            var allBookings = await GetAllBookingsAsync();
            
            return new Dictionary<string, object>
            {
                { "TotalBookings", allBookings.Count },
                { "PendingBookings", allBookings.Count(b => b.Status == "Pending") },
                { "ApprovedBookings", allBookings.Count(b => b.Status == "Approved") },
                { "RejectedBookings", allBookings.Count(b => b.Status == "Rejected") },
                { "CancelledBookings", allBookings.Count(b => b.Status == "Cancelled") }
            };
        }
    }
}
