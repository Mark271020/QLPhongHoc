using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace QLPhongHoc.Models
{
    [FirestoreData]
    public class User
    {
        public string Id { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string PasswordHash { get; set; }

        [FirestoreProperty]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "User";

        [FirestoreProperty]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [FirestoreProperty]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [FirestoreProperty]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; } = true;
    }
}