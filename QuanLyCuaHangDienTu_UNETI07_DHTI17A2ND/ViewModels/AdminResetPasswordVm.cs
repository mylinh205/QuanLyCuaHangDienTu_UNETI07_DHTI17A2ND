
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.ViewModels
{
    public class AdminResetPasswordVm
    {
        public int MaKhachHang { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu mới")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Mật khẩu phải từ 8 đến 100 ký tự")]
        [DataType(DataType.Password)]
        public string MatKhauMoi { get; set; } = "";

        [Required(ErrorMessage = "Xác nhận mật khẩu")]
        [Compare(nameof(MatKhauMoi),
            ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [DataType(DataType.Password)]
        public string XacNhanMatKhau { get; set; } = "";
    }
}
