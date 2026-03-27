using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace QLPhongHoc.Models
{
    [FirestoreData]
    public class Booking
    {
        public string Id { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Chọn phòng học")]
        public string RoomId { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Chọn người đặt")]
        public string UserId { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Ngày đặt là bắt buộc")]
        [Display(Name = "Ngày đặt")]
        public DateTime BookingDate { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Thời gian bắt đầu là bắt buộc")]
        [Display(Name = "Thời gian bắt đầu")]
        public DateTime StartTime { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Thời gian kết thúc là bắt buộc")]
        [Display(Name = "Thời gian kết thúc")]
        public DateTime EndTime { get; set; }

        [FirestoreProperty]
        [Display(Name = "Mục đích sử dụng")]
        public string Purpose { get; set; }

        [FirestoreProperty]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending";

        [FirestoreProperty]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
    }
}