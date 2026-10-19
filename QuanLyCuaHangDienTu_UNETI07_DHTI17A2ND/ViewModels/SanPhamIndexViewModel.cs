// Họ và tên: [Nguyễn Văn Dũng ]
// Mã sinh viên: [23203100024]
// Nội dung thực hiện: Module 2 - Quản lý sản phẩm, Tìm kiếm, Lọc, Sắp xếp và Phân trang dữ liệu
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.ViewModels
{
    public class SanPhamIndexViewModel
    {
        public List<SanPham> SanPhams { get; set; } = new List<SanPham>();

        // Điều kiện Tìm kiếm & Lọc
        public string? SearchTerm { get; set; }
        public int? MaLoai { get; set; }
        public string? ThuongHieu { get; set; }
        public string? TrangThai { get; set; }
        public decimal? GiaTu { get; set; }
        public decimal? GiaDen { get; set; }

        // Sắp xếp
        public string SortOrder { get; set; } = "name_asc";

        // Phân trang
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; } = 6;
    }
}