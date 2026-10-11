
using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.ViewModels
{
    public class AdminKhachHangEditVm
    {
        public int MaKhachHang { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        public string HoTen { get; set; } = "";

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        public string? GioiTinh { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        public string? DiaChi { get; set; }
    }
}
