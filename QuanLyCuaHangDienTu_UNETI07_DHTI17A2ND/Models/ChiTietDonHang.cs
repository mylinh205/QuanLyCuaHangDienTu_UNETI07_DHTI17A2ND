
// Họ và tên: Vũ Thành Đạt
// Mã sinh viên: 23203100069
// Nội dung thực hiện: Quản lý Đặt hàng & Xử lý Đơn hàng

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models
{
    public class ChiTietDonHang
    {
        public int MaDonHang { get; set; }

        public int MaSanPham { get; set; }

        [Required]
        [Range(1, int.MaxValue,
            ErrorMessage = "Số lượng phải lớn hơn 0.")]
        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Thành tiền")]
        public decimal ThanhTien { get; set; }

        [ForeignKey(nameof(MaDonHang))]
        public virtual DonHang? DonHang { get; set; }

        [ForeignKey(nameof(MaSanPham))]
        public virtual SanPham? SanPham { get; set; }
    }
}
