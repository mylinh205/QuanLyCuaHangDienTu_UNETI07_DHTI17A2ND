using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models
{
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được trống.")]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được trống.")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được trống.")]
        public string HoTen { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string VaiTro { get; set; } = "KhachHang"; // Admin | KhachHang
        public bool TrangThai { get; set; } = true;

        public virtual KhachHang? KhachHang { get; set; }
    }
}