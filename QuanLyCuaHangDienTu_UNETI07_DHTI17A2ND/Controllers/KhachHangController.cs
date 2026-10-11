
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Data;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.ViewModels;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly ApplicationDbContext _db;

        public KhachHangController(ApplicationDbContext db)
        {
            _db = db;
        }

        private bool IsAdmin =>
            HttpContext.Session.GetString("VaiTro") == "Admin";

        private bool IsKhachHang =>
            HttpContext.Session.GetString("VaiTro") == "KhachHang";

        private int? AccountId =>
            HttpContext.Session.GetInt32("MaTaiKhoan");

        private IActionResult AccessDenied()
        {
            if (AccountId == null)
                return RedirectToAction("Login", "Account");

            return StatusCode(403, "Bạn không có quyền truy cập.");
        }

        // ADMIN: Danh sách khách hàng
        [HttpGet]
        public async Task<IActionResult> Index(string? q)
        {
            if (!IsAdmin)
                return AccessDenied();

            var query = _db.KhachHangs
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();

                query = query.Where(x =>
                    (x.HoTen != null && x.HoTen.Contains(q)) ||
                    (x.Email != null && x.Email.Contains(q)) ||
                    (x.SoDienThoai != null &&
                     x.SoDienThoai.Contains(q)));
            }

            ViewBag.Q = q;

            var danhSach = await query
                .OrderBy(x => x.MaKhachHang)
                .ToListAsync();

            return View(danhSach);
        }

        // ADMIN: Chi tiết khách hàng
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!IsAdmin)
                return AccessDenied();

            var item = await _db.KhachHangs
                .Include(x => x.TaiKhoan)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaKhachHang == id);

            if (item == null)
                return NotFound();

            return View(item);
        }

        // ADMIN: Khóa / mở khóa khách hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            if (!IsAdmin)
                return AccessDenied();

            var item = await _db.KhachHangs
                .Include(x => x.TaiKhoan)
                .FirstOrDefaultAsync(x => x.MaKhachHang == id);

            if (item == null)
                return NotFound();

            item.TrangThai = !item.TrangThai;

            if (item.TaiKhoan != null)
                item.TaiKhoan.TrangThai = item.TrangThai;

            await _db.SaveChangesAsync();

            TempData["Success"] =
                "Đã cập nhật trạng thái khách hàng.";

            return RedirectToAction(nameof(Index));
        }

        // ADMIN: Mở trang sửa thông tin khách hàng
        [HttpGet]
        public async Task<IActionResult> AdminEdit(int id)
        {
            if (!IsAdmin)
                return AccessDenied();

            var item = await _db.KhachHangs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaKhachHang == id);

            if (item == null)
                return NotFound();

            var vm = new AdminKhachHangEditVm
            {
                MaKhachHang = item.MaKhachHang,
                HoTen = item.HoTen ?? "",
                NgaySinh = item.NgaySinh,
                GioiTinh = item.GioiTinh,
                SoDienThoai = item.SoDienThoai,
                Email = item.Email,
                DiaChi = item.DiaChi
            };

            return View(vm);
        }

        // ADMIN: Lưu thông tin khách hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEdit(
            AdminKhachHangEditVm vm)
        {
            if (!IsAdmin)
                return AccessDenied();

            if (vm.NgaySinh.HasValue &&
                (vm.NgaySinh.Value.Date > DateTime.Today ||
                 vm.NgaySinh.Value.Year < 1900))
            {
                ModelState.AddModelError(
                    nameof(vm.NgaySinh),
                    "Ngày sinh không hợp lệ.");
            }

            if (!ModelState.IsValid)
                return View(vm);

            var item = await _db.KhachHangs
                .FirstOrDefaultAsync(
                    x => x.MaKhachHang == vm.MaKhachHang);

            if (item == null)
                return NotFound();

            item.HoTen = vm.HoTen?.Trim();
            item.NgaySinh = vm.NgaySinh;
            item.GioiTinh = vm.GioiTinh;
            item.SoDienThoai = vm.SoDienThoai?.Trim();
            item.Email = vm.Email?.Trim();
            item.DiaChi = vm.DiaChi?.Trim();

            await _db.SaveChangesAsync();

            TempData["Success"] =
                "Cập nhật thông tin khách hàng thành công.";

            return RedirectToAction("Customers", "Admin");
        }

        // ADMIN: Mở trang đặt lại mật khẩu
        [HttpGet]
        public async Task<IActionResult> ResetPassword(int id)
        {
            if (!IsAdmin)
                return AccessDenied();

            var item = await _db.KhachHangs
                .Include(x => x.TaiKhoan)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaKhachHang == id);

            if (item == null)
                return NotFound();

            if (item.TaiKhoan == null)
                return BadRequest(
                    "Khách hàng chưa có tài khoản đăng nhập.");

            ViewBag.HoTen = item.HoTen;

            return View(new AdminResetPasswordVm
            {
                MaKhachHang = item.MaKhachHang
            });
        }

        // ADMIN: Lưu mật khẩu mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            AdminResetPasswordVm vm)
        {
            if (!IsAdmin)
                return AccessDenied();

            var item = await _db.KhachHangs
                .Include(x => x.TaiKhoan)
                .FirstOrDefaultAsync(
                    x => x.MaKhachHang == vm.MaKhachHang);

            if (item == null)
                return NotFound();

            if (item.TaiKhoan == null)
                return BadRequest(
                    "Khách hàng chưa có tài khoản đăng nhập.");

            if (item.TaiKhoan.VaiTro != "KhachHang")
                return StatusCode(403,
                    "Không được đặt lại mật khẩu tài khoản này.");

            if (!ModelState.IsValid)
            {
                ViewBag.HoTen = item.HoTen;
                return View(vm);
            }

            var hasher = new PasswordHasher<TaiKhoan>();

            item.TaiKhoan.MatKhau = hasher.HashPassword(
                item.TaiKhoan,
                vm.MatKhauMoi);

            await _db.SaveChangesAsync();

            TempData["Success"] =
                "Đặt lại mật khẩu khách hàng thành công.";

            return RedirectToAction("Customers", "Admin");
        }

        // KHÁCH HÀNG: Xem thông tin cá nhân
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (AccountId == null)
                return RedirectToAction("Login", "Account");

            if (!IsKhachHang)
                return AccessDenied();

            var item = await _db.KhachHangs
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.MaTaiKhoan == AccountId.Value);

            if (item == null)
                return NotFound("Chưa có hồ sơ khách hàng.");

            return View(item);
        }

        // KHÁCH HÀNG: Mở form cập nhật
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            if (AccountId == null)
                return RedirectToAction("Login", "Account");

            if (!IsKhachHang)
                return AccessDenied();

            var item = await _db.KhachHangs
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.MaTaiKhoan == AccountId.Value);

            if (item == null)
                return NotFound();

            var vm = new KhachHangEditVm
            {
                HoTen = item.HoTen,
                NgaySinh = item.NgaySinh,
                GioiTinh = item.GioiTinh,
                SoDienThoai = item.SoDienThoai,
                Email = item.Email,
                DiaChi = item.DiaChi
            };

            return View(vm);
        }

        // KHÁCH HÀNG: Lưu thông tin cá nhân
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(KhachHangEditVm vm)
        {
            if (AccountId == null)
                return RedirectToAction("Login", "Account");

            if (!IsKhachHang)
                return AccessDenied();

            if (vm.NgaySinh.HasValue &&
                (vm.NgaySinh.Value.Date > DateTime.Today ||
                 vm.NgaySinh.Value.Year < 1900))
            {
                ModelState.AddModelError(
                    nameof(vm.NgaySinh),
                    "Ngày sinh không hợp lệ.");
            }

            if (!ModelState.IsValid)
                return View(vm);

            var item = await _db.KhachHangs
                .FirstOrDefaultAsync(
                    x => x.MaTaiKhoan == AccountId.Value);

            if (item == null)
                return NotFound();

            item.HoTen = vm.HoTen?.Trim();
            item.NgaySinh = vm.NgaySinh;
            item.GioiTinh = vm.GioiTinh;
            item.SoDienThoai = vm.SoDienThoai;
            item.Email = vm.Email;
            item.DiaChi = vm.DiaChi;

            await _db.SaveChangesAsync();

            TempData["Success"] =
                "Cập nhật thông tin cá nhân thành công.";

            return RedirectToAction(nameof(Profile));
        }
    }
}
