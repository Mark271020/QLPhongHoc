using QLPhongHoc.Models;
using System.Threading.Tasks;

namespace QLPhongHoc.Services
{
    public static class SeedData
    {
        public static async Task InitializeAsync(FirebaseService firebaseService)
        {
            // Kiểm tra xem có admin user chưa
            var admin = await firebaseService.GetUserByUsernameAsync("admin");
            
            if (admin == null)
            {
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@qlphonghoc.com",
                    FullName = "Quản trị viên",
                    Role = "Admin",
                    PasswordHash = "Admin@123", // Sẽ được hash trong service
                    IsActive = true
                };
                
                await firebaseService.CreateUserAsync(adminUser);
                Console.WriteLine("✅ Đã tạo tài khoản Admin mặc định!");
                Console.WriteLine("   Username: admin");
                Console.WriteLine("   Password: Admin@123");
            }
        }
    }
}