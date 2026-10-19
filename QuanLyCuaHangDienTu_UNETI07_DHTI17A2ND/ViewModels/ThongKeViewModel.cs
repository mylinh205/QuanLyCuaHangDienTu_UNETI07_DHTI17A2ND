// Họ và tên: Vũ Thành Đạt
// Mã sinh viên: 23203100069
// Nội dung thực hiện: Module 4 - Quản lý đơn hàng, Xử lý trạng thái, Dashboard và Thống kê
namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.ViewModels
{
    public class ThongKeViewModel
    {
        // 1. Thống kê sản phẩm theo loại
        public List<SanPhamTheoLoaiDto> SanPhamTheoLoai { get; set; } = new();

        // 2. Thống kê đơn hàng theo trạng thái
        public List<DonHangTheoTrangThaiDto> DonHangTheoTrangThai { get; set; } = new();

        // 3. Top sản phẩm bán chạy (Đã hoàn thành)
        public List<SanPhamBanChayDto> SanPhamBanChay { get; set; } = new();

        // 4. Doanh thu theo tháng (Đã hoàn thành)
        public List<DoanhThuThangDto> DoanhThuTheoThang { get; set; } = new();

        // Các chỉ số tổng quát nhanh
        public decimal TongDoanhThuTatCa { get; set; }
        public int TongSoLuongDaBan { get; set; }
    }

    public class SanPhamTheoLoaiDto
    {
        public string TenLoai { get; set; } = string.Empty;
        public int SoLuongSanPham { get; set; }
    }

    public class DonHangTheoTrangThaiDto
    {
        public string TrangThai { get; set; } = string.Empty;
        public int SoLuongDon { get; set; }
    }

    public class SanPhamBanChayDto
    {
        public string TenSanPham { get; set; } = string.Empty;
        public int TongSoLuongBan { get; set; }
        public decimal TongDoanhThu { get; set; }
    }

    public class DoanhThuThangDto
    {
        public string ThangNam { get; set; } = string.Empty;
        public decimal DoanhThu { get; set; }
    }
}