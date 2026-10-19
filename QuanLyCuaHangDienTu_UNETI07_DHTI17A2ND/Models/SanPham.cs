// Họ và tên: [Nguyễn Văn Dũng ]
// Mã sinh viên: [23203100024]
// Nội dung thực hiện: Module 2 - Thiết kế Entity SanPham và Data Annotations
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models
{
    public class SanPham
    {
        [Key]
        public int MaSanPham { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm bắt buộc nhập.")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không vượt quá 200 ký tự.")]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm.")]
        [Display(Name = "Loại sản phẩm")]
        public int MaLoai { get; set; }

        [Required(ErrorMessage = "Thương hiệu bắt buộc nhập.")]
        [StringLength(100, ErrorMessage = "Thương hiệu không vượt quá 100 ký tự.")]
        [Display(Name = "Thương hiệu")]
        public string ThuongHieu { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Xuất xứ")]
        public string? XuatXu { get; set; }

        [StringLength(500, ErrorMessage = "Đường dẫn ảnh không vượt quá 500 ký tự.")]
        [Display(Name = "Ảnh sản phẩm")]
        public string? HinhAnh { get; set; }

        [Required(ErrorMessage = "Đơn giá bắt buộc nhập.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn 0.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }

        [Required(ErrorMessage = "Số lượng tồn bắt buộc nhập.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải lớn hơn hoặc bằng 0.")]
        [Display(Name = "Số lượng tồn")]
        public int SoLuongTon { get; set; }

        [Display(Name = "Thời gian bảo hành (tháng)")]
        [Range(0, 120, ErrorMessage = "Thời gian bảo hành không hợp lệ.")]
        public int ThoiGianBaoHanh { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Trạng thái bắt buộc chọn.")]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Đang kinh doanh"; // Đang kinh doanh, Tạm ngừng, Ngừng kinh doanh

        [ForeignKey("MaLoai")]
        public virtual LoaiSanPham? LoaiSanPham { get; set; }
    }
}