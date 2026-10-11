
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models;

public class DonHang
{
    [Key]
    public int MaDonHang { get; set; }

    [Required]
    public int MaKhachHang { get; set; }

    public DateTime NgayDat { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
    [StringLength(255)]
    public string DiaChiGiaoHang { get; set; } = "";

    [Column(TypeName = "decimal(18,2)")]
    public decimal TongTien { get; set; }

    [Required]
    [StringLength(30)]
    public string TrangThai { get; set; } = "Chờ xác nhận";

    [ForeignKey(nameof(MaKhachHang))]
    public virtual KhachHang KhachHang { get; set; } = null!;

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs
    { get; set; } = new List<ChiTietDonHang>();
}
