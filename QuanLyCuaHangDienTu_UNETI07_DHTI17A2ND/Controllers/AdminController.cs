
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Data;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models;
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

        private bool IsAdmin =>
            HttpContext.Session.GetInt32("MaTaiKhoan").HasValue &&
            HttpContext.Session.GetString("VaiTro") == "Admin";

        // =====================================
        // DASHBOARD
        // =====================================
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin)
                return RedirectToAction("Login", "Account");

            ViewBag.ProductCount =
                await _context.SanPhams.CountAsync();

            ViewBag.CategoryCount =
                await _context.LoaiSanPhams.CountAsync();

            ViewBag.AccountCount =
                await _context.TaiKhoans.CountAsync();

            ViewBag.CustomerCount =
                await _context.KhachHangs.CountAsync();

            return View();
        }

        // =====================================
        // DANH SÁCH VÀ TÌM KIẾM KHÁCH HÀNG
        // =====================================
        [HttpGet]
        public async Task<IActionResult> Customers(string? q)
        {
            if (!IsAdmin)
                return RedirectToAction("Login", "Account");

            var query = _context.KhachHangs
                .Include(x => x.TaiKhoan)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();

                query = query.Where(x =>
                    (x.HoTen != null && x.HoTen.Contains(q)) ||
                    (x.Email != null && x.Email.Contains(q)) ||
                    (x.SoDienThoai != null &&
                     x.SoDienThoai.Contains(q)) ||
                    (x.TaiKhoan != null &&
                     x.TaiKhoan.TenDangNhap.Contains(q)));
            }

            ViewBag.Q = q;

            var customers = await query
                .OrderBy(x => x.HoTen)
                .ToListAsync();

            return View(customers);
        }

        // =====================================
        // KHÓA / MỞ KHÓA KHÁCH HÀNG
        // =====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCustomerStatus(int id)
        {
            if (!IsAdmin)
                return Forbid();

            var customer = await _context.KhachHangs
                .Include(x => x.TaiKhoan)
                .FirstOrDefaultAsync(x => x.MaKhachHang == id);

            if (customer == null)
                return NotFound();

            customer.TrangThai = !customer.TrangThai;

            if (customer.TaiKhoan != null)
            {
                customer.TaiKhoan.TrangThai =
                    customer.TrangThai;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = customer.TrangThai
                ? "Đã mở khóa tài khoản khách hàng."
                : "Đã khóa tài khoản khách hàng.";

            return RedirectToAction(nameof(Customers));
        }

        // =====================================
        // SỬA THÔNG TIN KHÁCH HÀNG
        // =====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEdit(
            AdminKhachHangEditVm vm)
        {
            if (!IsAdmin)
                return Forbid();

            if (vm.NgaySinh.HasValue &&
                (vm.NgaySinh.Value.Date > DateTime.Today ||
                 vm.NgaySinh.Value.Year < 1900))
            {
                ModelState.AddModelError(
                    nameof(vm.NgaySinh),
                    "Ngày sinh không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ",
                    ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage));

                return RedirectToAction(nameof(Customers));
            }

            var customer = await _context.KhachHangs
                .FirstOrDefaultAsync(
                    x => x.MaKhachHang == vm.MaKhachHang);

            if (customer == null)
                return NotFound();

            customer.HoTen = vm.HoTen.Trim();
            customer.NgaySinh = vm.NgaySinh;
            customer.GioiTinh = vm.GioiTinh?.Trim();
            customer.SoDienThoai = vm.SoDienThoai?.Trim();
            customer.Email = vm.Email?.Trim();
            customer.DiaChi = vm.DiaChi?.Trim();

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Cập nhật thông tin khách hàng thành công.";

            return RedirectToAction(nameof(Customers));
        }

        // =====================================
        // ĐẶT LẠI MẬT KHẨU KHÁCH HÀNG
        // =====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            AdminResetPasswordVm vm)
        {
            if (!IsAdmin)
                return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ",
                    ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage));

                return RedirectToAction(nameof(Customers));
            }

            var customer = await _context.KhachHangs
                .Include(x => x.TaiKhoan)
                .FirstOrDefaultAsync(
                    x => x.MaKhachHang == vm.MaKhachHang);

            if (customer == null ||
                customer.TaiKhoan == null)
            {
                TempData["Error"] =
                    "Không tìm thấy tài khoản khách hàng.";

                return RedirectToAction(nameof(Customers));
            }

            // Không cho phép đặt lại mật khẩu Admin
            // qua chức năng quản lý khách hàng.
            if (customer.TaiKhoan.VaiTro != "KhachHang")
                return Forbid();

            var hasher = new PasswordHasher<TaiKhoan>();

            customer.TaiKhoan.MatKhau =
                hasher.HashPassword(
                    customer.TaiKhoan,
                    vm.MatKhauMoi);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Đặt lại mật khẩu khách hàng thành công.";

            return RedirectToAction(nameof(Customers));
        }
    }
}
