using System.ComponentModel.DataAnnotations;

namespace QLPhongHoc.Models
{
    public class PhongHoc
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Tên phòng học là bắt buộc")]
        [Display(Name = "Tên phòng học")]
        public string TenPhong { get; set; }

        [Required(ErrorMessage = "Vị trí là bắt buộc")]
        [Display(Name = "Vị trí")]
        public string ViTri { get; set; }

        [Required(ErrorMessage = "Sức chứa là bắt buộc")]
        [Display(Name = "Sức chứa")]
        [Range(1, 500)]
        public int SucChua { get; set; }

        [Display(Name = "Trang thiết bị")]
        public string TrangThietBi { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}