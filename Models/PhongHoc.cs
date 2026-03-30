using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace QLPhongHoc.Models
{
    [FirestoreData]
    public class PhongHoc
    {
        // KHÔNG có [Required] ở đây
        public string Id { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Tên phòng học là bắt buộc")]
        [Display(Name = "Tên phòng học")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên phòng học phải từ 3-100 ký tự")]
        public string TenPhong { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Vị trí là bắt buộc")]
        [Display(Name = "Vị trí")]
        public string ViTri { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Sức chứa là bắt buộc")]
        [Display(Name = "Sức chứa")]
        [Range(1, 500, ErrorMessage = "Sức chứa phải từ 1-500")]
        public int SucChua { get; set; }

        [FirestoreProperty]
        [Display(Name = "Trang thiết bị")]
        public string TrangThietBi { get; set; }

        [FirestoreProperty]
        [Display(Name = "Trạng thái")]
        public bool IsAvailable { get; set; } = true;

        [FirestoreProperty]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }

        [FirestoreProperty]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}