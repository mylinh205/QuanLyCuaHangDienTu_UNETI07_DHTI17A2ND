
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models
{
    public class KhachHang
    {
        [Key]
        public int MaKhachHang { get; set; }

        public int? MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        public string? HoTen { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        public string? GioiTinh { get; set; }

        [RegularExpression(@"^0[0-9]{9}$",
            ErrorMessage = "Số điện thoại phải có 10 chữ số")]
        public string? SoDienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        public DateTime NgayDangKy { get; set; }
            = DateTime.Now;

        public bool TrangThai { get; set; } = true;

        // Quan hệ với tài khoản
        [ForeignKey(nameof(MaTaiKhoan))]
        public virtual TaiKhoan? TaiKhoan { get; set; }

        // Quan hệ với đơn hàng
        public virtual ICollection<DonHang> DonHangs
        { get; set; } = new List<DonHang>();
    }
}
