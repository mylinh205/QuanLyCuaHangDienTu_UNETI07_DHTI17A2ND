using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models
{
    public class LoaiSanPham
    {
        [Key]
        public int MaLoai { get; set; }

        [Required(ErrorMessage = "Tên loại sản phẩm không được để trống.")]
        [StringLength(100)]
        [Display(Name = "Tên loại sản phẩm")]
        public string TenLoai { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}