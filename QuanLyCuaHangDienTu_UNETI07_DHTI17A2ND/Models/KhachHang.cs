using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models
{
    // SV Thực hiện: Đào Minh Long - MSV: 23203100068
    public class KhachHang
    {
        [Key]
        public int MaKhachHang { get; set; }

        public int? MaTaiKhoan { get; set; }

        public string? HoTen { get; set; }

        public string? SoDienThoai { get; set; }

        public string? Email { get; set; }

        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true;

        // Relationship
        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}