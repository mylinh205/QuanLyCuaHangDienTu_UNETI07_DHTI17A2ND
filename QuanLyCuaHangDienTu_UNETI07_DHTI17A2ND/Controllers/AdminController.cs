
// Họ và tên: Vũ Thành Đạt
// Mã sinh viên: 23203100069
// Nội dung thực hiện: Module 4 - Quản lý đơn hàng,
// Xử lý trạng thái, Dashboard, Thống kê
// Tích hợp chức năng Quản lý khách hàng

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        // ============================================
        // KIỂM TRA QUYỀN ADMIN
        // ============================================
        private bool CheckAdmin()
        {
            return HttpContext.Session.GetString("VaiTro") == "Admin";
        }

        // ============================================
        // 1. DASHBOARD QUẢN TRỊ
        // GET: /Admin/Dashboard
        // ============================================
        public async Task<IActionResult> Dashboard()
        {
            if (!CheckAdmin())
                return RedirectToAction("Login", "Account");

            var model = new DashboardViewModel
            {
                TongLoaiSanPham =
                    await _context.LoaiSanPhams.CountAsync(),

                TongSanPham =
                    await _context.SanPhams.CountAsync(),

                TongKhachHang =
                    await _context.KhachHangs.CountAsync(),

                TongDonHang =
                    await _context.DonHangs.CountAsync(),

                DonHangChoXacNhan =
                    await _context.DonHangs.CountAsync(
                        d => d.TrangThai == "Chờ xác nhận"),

                DonHangDangGiao =
                    await _context.DonHangs.CountAsync(
                        d => d.TrangThai == "Đang giao"),

                DonHangDaHoanThanh =
                    await _context.DonHangs.CountAsync(
                        d => d.TrangThai == "Đã hoàn thành"),

                SanPhamDangKinhDoanh =
                    await _context.SanPhams.CountAsync(
                        s => s.TrangThai == "Đang kinh doanh")
            };

            return View(model);
        }

        // ============================================
        // 2. QUẢN LÝ TOÀN BỘ ĐƠN HÀNG
        // GET: /Admin/QuanLyDonHang
        // ============================================
        public async Task<IActionResult> QuanLyDonHang(
            string? trangThai)
        {
            if (!CheckAdmin())
                return RedirectToAction("Login", "Account");

            var query = _context.DonHangs
                .Include(d => d.KhachHang)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(
                    d => d.TrangThai == trangThai);
            }

            var danhSachDonHang = await query
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();

            ViewBag.TrangThaiHienTai = trangThai;

            return View(danhSachDonHang);
        }

        // ============================================
        // 3. CHI TIẾT ĐƠN HÀNG
        // GET: /Admin/ChiTietDonHang/5
        // ============================================
        public async Task<IActionResult> ChiTietDonHang(int? id)
        {
            if (!CheckAdmin())
                return RedirectToAction("Login", "Account");

            if (id == null)
                return NotFound();

            var donHang = await _context.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(
                    d => d.MaDonHang == id);

            if (donHang == null)
                return NotFound();

            return View(donHang);
        }

        // ============================================
        // 4. CẬP NHẬT TRẠNG THÁI ĐƠN HÀNG
        // POST: /Admin/CapNhatTrangThai
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(
            int maDonHang,
            string trangThaiMoi)
        {
            if (!CheckAdmin())
                return RedirectToAction("Login", "Account");

            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(
                    d => d.MaDonHang == maDonHang);

            if (donHang == null)
                return NotFound();

            string trangThaiCu = donHang.TrangThai;

            // Các trạng thái được phép sử dụng
            var trangThaiHopLe = new[]
            {
                "Chờ xác nhận",
                "Đã xác nhận",
                "Đang giao",
                "Đã hoàn thành",
                "Đã hủy"
            };

            if (string.IsNullOrWhiteSpace(trangThaiMoi) ||
                !trangThaiHopLe.Contains(trangThaiMoi))
            {
                TempData["Error"] =
                    "Trạng thái đơn hàng không hợp lệ.";

                return RedirectToAction(
                    nameof(ChiTietDonHang),
                    new { id = maDonHang });
            }

            if (trangThaiCu == trangThaiMoi)
            {
                TempData["Error"] =
                    "Đơn hàng đã ở trạng thái này.";

                return RedirectToAction(
                    nameof(ChiTietDonHang),
                    new { id = maDonHang });
            }

            // Chặn thay đổi đơn đã hoàn thành hoặc đã hủy
            if (trangThaiCu == "Đã hoàn thành" ||
                trangThaiCu == "Đã hủy")
            {
                TempData["Error"] =
                    "Không thể thay đổi trạng thái đơn hàng " +
                    "đã hoàn thành hoặc đã hủy.";

                return RedirectToAction(
                    nameof(ChiTietDonHang),
                    new { id = maDonHang });
            }

            // Kiểm tra chuyển trạng thái hợp lệ
            bool chuyenHopLe =
                (trangThaiCu == "Chờ xác nhận" &&
                 (trangThaiMoi == "Đã xác nhận" ||
                  trangThaiMoi == "Đã hủy")) ||

                (trangThaiCu == "Đã xác nhận" &&
                 (trangThaiMoi == "Đang giao" ||
                  trangThaiMoi == "Đã hủy")) ||

                (trangThaiCu == "Đang giao" &&
                 (trangThaiMoi == "Đã hoàn thành" ||
                  trangThaiMoi == "Đã hủy"));

            if (!chuyenHopLe)
            {
                TempData["Error"] =
                    "Không thể chuyển từ trạng thái '" +
                    trangThaiCu + "' sang '" +
                    trangThaiMoi + "'.";

                return RedirectToAction(
                    nameof(ChiTietDonHang),
                    new { id = maDonHang });
            }

            // Trừ kho khi xác nhận đơn hàng
            if (trangThaiCu == "Chờ xác nhận" &&
                trangThaiMoi == "Đã xác nhận")
            {
                foreach (var item in donHang.ChiTietDonHangs)
                {
                    var sp = item.SanPham;

                    if (sp == null ||
                        sp.SoLuongTon < item.SoLuong)
                    {
                        TempData["Error"] =
                            $"Sản phẩm '{sp?.TenSanPham ?? "Không xác định"}' " +
                            "không đủ số lượng tồn kho.";

                        return RedirectToAction(
                            nameof(ChiTietDonHang),
                            new { id = maDonHang });
                    }
                }

                foreach (var item in donHang.ChiTietDonHangs)
                {
                    if (item.SanPham != null)
                    {
                        item.SanPham.SoLuongTon -= item.SoLuong;
                    }
                }
            }

            // Hoàn kho nếu hủy đơn đã xác nhận/đang giao
            if (trangThaiMoi == "Đã hủy" &&
                (trangThaiCu == "Đã xác nhận" ||
                 trangThaiCu == "Đang giao"))
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

            TempData["Success"] =
                $"Đã cập nhật đơn hàng #{maDonHang} " +
                $"thành '{trangThaiMoi}' thành công!";

            return RedirectToAction(
                nameof(ChiTietDonHang),
                new { id = maDonHang });
        }

        // ============================================
        // 5. THỐNG KÊ DỮ LIỆU VÀ DOANH THU
        // GET: /Admin/ThongKe
        // ============================================
        public async Task<IActionResult> ThongKe()
        {
            if (!CheckAdmin())
                return RedirectToAction("Login", "Account");

            var viewModel = new ThongKeViewModel();

            // Thống kê sản phẩm theo loại
            viewModel.SanPhamTheoLoai =
                await _context.SanPhams
                    .GroupBy(s => s.LoaiSanPham!.TenLoai)
                    .Select(g => new SanPhamTheoLoaiDto
                    {
                        TenLoai = g.Key ?? "Chưa phân loại",
                        SoLuongSanPham = g.Count()
                    })
                    .ToListAsync();

            // Thống kê đơn hàng theo trạng thái
            viewModel.DonHangTheoTrangThai =
                await _context.DonHangs
                    .GroupBy(d => d.TrangThai)
                    .Select(g => new DonHangTheoTrangThaiDto
                    {
                        TrangThai = g.Key ?? "Không xác định",
                        SoLuongDon = g.Count()
                    })
                    .ToListAsync();

            // Top 5 sản phẩm bán chạy
            viewModel.SanPhamBanChay =
                await _context.ChiTietDonHangs
                    .Where(c =>
                        c.DonHang!.TrangThai == "Đã hoàn thành")
                    .GroupBy(c => c.SanPham!.TenSanPham)
                    .Select(g => new SanPhamBanChayDto
                    {
                        TenSanPham = g.Key ?? "Không rõ",
                        TongSoLuongBan =
                            g.Sum(c => c.SoLuong),
                        TongDoanhThu =
                            g.Sum(c => c.ThanhTien)
                    })
                    .OrderByDescending(g => g.TongSoLuongBan)
                    .Take(5)
                    .ToListAsync();

            // Doanh thu theo tháng
            var rawDoanhThu =
                await _context.DonHangs
                    .Where(d =>
                        d.TrangThai == "Đã hoàn thành")
                    .GroupBy(d => new
                    {
                        d.NgayDat.Year,
                        d.NgayDat.Month
                    })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        DoanhThu =
                            g.Sum(d => d.TongTien)
                    })
                    .OrderBy(g => g.Year)
                    .ThenBy(g => g.Month)
                    .ToListAsync();

            viewModel.DoanhThuTheoThang =
                rawDoanhThu.Select(x => new DoanhThuThangDto
                {
                    ThangNam = $"{x.Month:D2}/{x.Year}",
                    DoanhThu = x.DoanhThu
                }).ToList();

            viewModel.TongDoanhThuTatCa =
                viewModel.DoanhThuTheoThang
                    .Sum(x => x.DoanhThu);

            viewModel.TongSoLuongDaBan =
                viewModel.SanPhamBanChay
                    .Sum(x => x.TongSoLuongBan);

            return View(viewModel);
        }

        // ============================================
        // 6. QUẢN LÝ KHÁCH HÀNG
        // GET: /Admin/Customers
        // ============================================
        public async Task<IActionResult> Customers(string? q)
        {
            if (!CheckAdmin())
                return RedirectToAction("Login", "Account");

            var query = _context.KhachHangs
                .Include(k => k.TaiKhoan)
                .AsNoTracking()
                .AsQueryable();

            // Tìm kiếm theo họ tên, email,
            // số điện thoại và tên đăng nhập
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();

                query = query.Where(k =>
                    (k.HoTen != null &&
                     k.HoTen.Contains(q)) ||

                    (k.Email != null &&
                     k.Email.Contains(q)) ||

                    (k.SoDienThoai != null &&
                     k.SoDienThoai.Contains(q)) ||

                    (k.TaiKhoan != null &&
                     k.TaiKhoan.TenDangNhap.Contains(q)));
            }

            ViewBag.Q = q;

            var customers = await query
                .OrderBy(k => k.MaKhachHang)
                .ToListAsync();

            return View(customers);
        }

        // ============================================
        // 7. KHÓA / MỞ KHÓA KHÁCH HÀNG
        // POST: /Admin/ToggleCustomerStatus
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCustomerStatus(int id)
        {
            if (!CheckAdmin())
                return RedirectToAction("Login", "Account");

            var customer = await _context.KhachHangs
                .Include(k => k.TaiKhoan)
                .FirstOrDefaultAsync(
                    k => k.MaKhachHang == id);

            if (customer == null)
            {
                TempData["Error"] =
                    "Không tìm thấy khách hàng.";

                return RedirectToAction(nameof(Customers));
            }

            bool trangThaiMoi = !customer.TrangThai;

            // Cập nhật trạng thái khách hàng
            customer.TrangThai = trangThaiMoi;

            // Đồng bộ với trạng thái tài khoản
            if (customer.TaiKhoan != null)
            {
                customer.TaiKhoan.TrangThai = trangThaiMoi;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = trangThaiMoi
                ? $"Đã mở khóa khách hàng '{customer.HoTen}'."
                : $"Đã khóa khách hàng '{customer.HoTen}'.";

            return RedirectToAction(nameof(Customers));
        }
    }
}
