// Họ và tên: Vũ Thành Đạt
// Mã sinh viên: 23203100069
// Nội dung thực hiện: Module 4 - Quản lý đơn hàng, Dashboard, Thống kê dữ liệu bằng LINQ
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Data;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.ViewModels;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool CheckAdmin()
        {
            return HttpContext.Session.GetString("VaiTro") == "Admin";
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            var model = new DashboardViewModel
            {
                TongLoaiSanPham = await _context.LoaiSanPhams.CountAsync(),
                TongSanPham = await _context.SanPhams.CountAsync(),
                TongKhachHang = await _context.KhachHangs.CountAsync(),
                TongDonHang = await _context.DonHangs.CountAsync(),
                DonHangChoXacNhan = await _context.DonHangs.CountAsync(d => d.TrangThai == "Chờ xác nhận"),
                DonHangDangGiao = await _context.DonHangs.CountAsync(d => d.TrangThai == "Đang giao"),
                DonHangDaHoanThanh = await _context.DonHangs.CountAsync(d => d.TrangThai == "Đã hoàn thành"),
                SanPhamDangKinhDoanh = await _context.SanPhams.CountAsync(s => s.TrangThai == "Đang kinh doanh")
            };

            return View(model);
        }

        // GET: Admin/ThongKe
        public async Task<IActionResult> ThongKe()
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            // 1. Thống kê số sản phẩm theo từng loại (LINQ GroupBy & Count)
            ViewBag.SanPhamTheoLoai = await _context.SanPhams
                .GroupBy(s => s.LoaiSanPham!.TenLoai)
                .Select(g => new { TenLoai = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            // 2. Thống kê số đơn hàng theo từng trạng thái
            ViewBag.DonHangTheoTrangThai = await _context.DonHangs
                .GroupBy(d => d.TrangThai)
                .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            // 3. Thống kê số lượng bán & Doanh thu theo sản phẩm (Chỉ tính đơn Đã hoàn thành)
            ViewBag.SoLuongBanTheoSanPham = await _context.ChiTietDonHangs
                .Where(c => c.DonHang!.TrangThai == "Đã hoàn thành")
                .GroupBy(c => c.SanPham!.TenSanPham)
                .Select(g => new {
                    TenSanPham = g.Key,
                    TongSoLuong = g.Sum(c => c.SoLuong),
                    TongDoanhThu = g.Sum(c => c.ThanhTien)
                })
                .OrderByDescending(g => g.TongSoLuong)
                .ToListAsync();

            // 4. Doanh thu theo tháng
            ViewBag.DoanhThuTheoThang = await _context.DonHangs
                .Where(d => d.TrangThai == "Đã hoàn thành")
                .GroupBy(d => new { d.NgayDat.Year, d.NgayDat.Month })
                .Select(g => new {
                    Thang = $"{g.Key.Month}/{g.Key.Year}",
                    DoanhThu = g.Sum(d => d.TongTien)
                })
                .ToListAsync();

            return View();
        }
    }
}