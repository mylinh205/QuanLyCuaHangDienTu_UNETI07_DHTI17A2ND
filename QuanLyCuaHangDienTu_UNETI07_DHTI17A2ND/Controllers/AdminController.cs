// Họ và tên: Vũ Thành Đạt
// Mã sinh viên: 23203100069
// Nội dung thực hiện: Module 4 - Quản lý đơn hàng, Xử lý trạng thái, Dashboard và Thống kê
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

        // Kiểm tra quyền Admin qua Session
        private bool CheckAdmin()
        {
            return HttpContext.Session.GetString("VaiTro") == "Admin";
        }

        // ================= 1. DASHBOARD QUẢN TRỊ =================
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

        // ================= 2. QUẢN LÝ ĐƠN HÀNG =================
        // GET: Admin/QuanLyDonHang
        public async Task<IActionResult> QuanLyDonHang(string? trangThai)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            var query = _context.DonHangs
                .Include(d => d.KhachHang)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(d => d.TrangThai == trangThai);
            }

            var danhSachDonHang = await query.OrderByDescending(d => d.NgayDat).ToListAsync();
            ViewBag.TrangThaiHienTai = trangThai;
            return View(danhSachDonHang);
        }

        // GET: Admin/ChiTietDonHang/5
        public async Task<IActionResult> ChiTietDonHang(int? id)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var donHang = await _context.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);

            if (donHang == null) return NotFound();

            return View(donHang);
        }

        // POST: Admin/CapNhatTrangThai
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int maDonHang, string trangThaiMoi)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(d => d.MaDonHang == maDonHang);

            if (donHang == null) return NotFound();

            string trangThaiCu = donHang.TrangThai;

            // 1. Nghiệp vụ: Chặn không cho chuyển ngược từ "Đã hoàn thành" về "Chờ xác nhận"
            if (trangThaiCu == "Đã hoàn thành" && trangThaiMoi == "Chờ xác nhận")
            {
                TempData["Error"] = "Đơn hàng đã hoàn thành không được phép chuyển ngược về chờ xác nhận!";
                return RedirectToAction(nameof(ChiTietDonHang), new { id = maDonHang });
            }

            // 2. Nghiệp vụ kiểm tra tồn kho & Trừ kho khi chuyển từ "Chờ xác nhận" sang "Đã xác nhận"
            if (trangThaiCu == "Chờ xác nhận" && trangThaiMoi == "Đã xác nhận")
            {
                foreach (var item in donHang.ChiTietDonHangs)
                {
                    var sp = item.SanPham;
                    if (sp == null || sp.SoLuongTon < item.SoLuong)
                    {
                        TempData["Error"] = $"Sản phẩm '{sp?.TenSanPham ?? "Không xác định"}' không đủ số lượng tồn kho (Còn lại: {sp?.SoLuongTon ?? 0}, Cần: {item.SoLuong}). Không thể xác nhận đơn hàng!";
                        return RedirectToAction(nameof(ChiTietDonHang), new { id = maDonHang });
                    }
                }

                // Đủ hàng -> Tiến hành trừ số lượng tồn kho
                foreach (var item in donHang.ChiTietDonHangs)
                {
                    item.SanPham.SoLuongTon -= item.SoLuong;
                    if (item.SanPham.SoLuongTon < 0) item.SanPham.SoLuongTon = 0; // Ràng buộc SoLuongTon >= 0
                }
            }

            // 3. Nghiệp vụ: Nếu hủy đơn mà trước đó đã trừ kho (ví dụ từ Đã xác nhận / Đang giao -> Đã hủy), hoàn lại số lượng tồn kho
            if (trangThaiMoi == "Đã hủy" && (trangThaiCu == "Đã xác nhận" || trangThaiCu == "Đang giao"))
            {
                foreach (var item in donHang.ChiTietDonHangs)
                {
                    if (item.SanPham != null)
                    {
                        item.SanPham.SoLuongTon += item.SoLuong;
                    }
                }
            }

            donHang.TrangThai = trangThaiMoi;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã cập nhật trạng thái đơn hàng #{maDonHang} thành '{trangThaiMoi}' thành công!";
            return RedirectToAction(nameof(ChiTietDonHang), new { id = maDonHang });
        }

        // ================= 3. THỐNG KÊ DỮ LIỆU & BÁO CÁO LINQ =================
        public async Task<IActionResult> ThongKe()
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            // 1. Thống kê số lượng sản phẩm theo từng loại (GROUP BY)
            ViewBag.SanPhamTheoLoai = await _context.SanPhams
                .GroupBy(s => s.LoaiSanPham!.TenLoai)
                .Select(g => new { TenLoai = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            // 2. Thống kê đơn hàng theo trạng thái
            ViewBag.DonHangTheoTrangThai = await _context.DonHangs
                .GroupBy(d => d.TrangThai)
                .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            // 3. Thống kê sản phẩm bán chạy nhất (Chỉ tính đơn đã hoàn thành)
            ViewBag.SanPhamBanChay = await _context.ChiTietDonHangs
                .Where(c => c.DonHang!.TrangThai == "Đã hoàn thành")
                .GroupBy(c => c.SanPham!.TenSanPham)
                .Select(g => new {
                    TenSanPham = g.Key,
                    TongSoLuongBan = g.Sum(c => c.SoLuong),
                    TongDoanhThu = g.Sum(c => c.ThanhTien)
                })
                .OrderByDescending(g => g.TongSoLuongBan)
                .Take(5)
                .ToListAsync();

            // 4. Thống kê doanh thu theo tháng
            var thongKeThangRaw = await _context.DonHangs
                .Where(d => d.TrangThai == "Đã hoàn thành")
                .GroupBy(d => new { d.NgayDat.Year, d.NgayDat.Month })
                .Select(g => new {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TongDoanhThu = g.Sum(d => d.TongTien)
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToListAsync();

            ViewBag.DoanhThuTheoThang = thongKeThangRaw
                .Select(g => new {
                    ThangNam = $"{g.Month:D2}/{g.Year}",
                    TongDoanhThu = g.TongDoanhThu
                })
                .ToList();

            return View();
        }
    }
}