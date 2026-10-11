// Họ và tên: [Đào Minh Long]
// Mã sinh viên: [23203100068]
// Nội dung: Module 1 - Quản lý tài khoản & Xác thực
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Data;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.ViewModels;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<TaiKhoan> _passwordHasher = new();

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login() => View();

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            model.TenDangNhap = model.TenDangNhap.Trim();
            var user = await _context.TaiKhoans
                .FirstOrDefaultAsync(u => u.TenDangNhap == model.TenDangNhap);

            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model);
            }

            if (!user.TrangThai)
            {
                ModelState.AddModelError("", "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên.");
                return View(model);
            }

            if (user.VaiTro is not ("Admin" or "KhachHang"))
            {
                ModelState.AddModelError("", "Vai trò tài khoản không hợp lệ.");
                return View(model);
            }

            var hasLegacyPassword = !user.MatKhau.StartsWith("AQAAAA", StringComparison.Ordinal);
            var passwordIsValid = hasLegacyPassword
                ? string.Equals(user.MatKhau, model.MatKhau, StringComparison.Ordinal)
                : _passwordHasher.VerifyHashedPassword(user, user.MatKhau, model.MatKhau) != PasswordVerificationResult.Failed;

            if (!passwordIsValid)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model);
            }

            if (hasLegacyPassword)
            {
                user.MatKhau = _passwordHasher.HashPassword(user, model.MatKhau);
                await _context.SaveChangesAsync();
            }

            // Lưu Session
            HttpContext.Session.Clear();
            HttpContext.Session.SetInt32("MaTaiKhoan", user.MaTaiKhoan);
            HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
            HttpContext.Session.SetString("HoTen", user.HoTen);
            HttpContext.Session.SetString("VaiTro", user.VaiTro);

            if (user.VaiTro == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Register
        public IActionResult Register() => View();

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (HttpContext.Session.GetString("VaiTro") != "KhachHang")
                return RedirectToAction("Login");

            var accountId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var customer = await _context.KhachHangs
                .FirstOrDefaultAsync(item => item.MaTaiKhoan == accountId);

            if (customer == null) return NotFound();

            return View(new CustomerProfileViewModel
            {
                HoTen = customer.HoTen ?? string.Empty,
                Email = customer.Email ?? string.Empty,
                SoDienThoai = customer.SoDienThoai ?? string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(CustomerProfileViewModel model)
        {
            if (HttpContext.Session.GetString("VaiTro") != "KhachHang")
                return RedirectToAction("Login");

            if (!ModelState.IsValid) return View(model);

            var accountId = HttpContext.Session.GetInt32("MaTaiKhoan");
            var customer = await _context.KhachHangs
                .Include(item => item.TaiKhoan)
                .FirstOrDefaultAsync(item => item.MaTaiKhoan == accountId);

            if (customer == null) return NotFound();

            customer.HoTen = model.HoTen.Trim();
            customer.Email = model.Email.Trim();
            customer.SoDienThoai = model.SoDienThoai.Trim();
            if (customer.TaiKhoan != null)
            {
                customer.TaiKhoan.HoTen = customer.HoTen;
                customer.TaiKhoan.Email = customer.Email;
            }

            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("HoTen", customer.HoTen);
            TempData["Success"] = "Cập nhật thông tin cá nhân thành công.";
            return RedirectToAction(nameof(Profile));
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            model.TenDangNhap = model.TenDangNhap.Trim();
            model.Email = model.Email.Trim();
            if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == model.TenDangNhap))
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }

            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap,
                HoTen = model.HoTen,
                Email = model.Email,
                VaiTro = "KhachHang",
                TrangThai = true
            };
            taiKhoan.MatKhau = _passwordHasher.HashPassword(taiKhoan, model.MatKhau);

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.TaiKhoans.Add(taiKhoan);
                await _context.SaveChangesAsync();

                var khachHang = new KhachHang
                {
                    MaTaiKhoan = taiKhoan.MaTaiKhoan,
                    HoTen = model.HoTen,
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    NgayDangKy = DateTime.Now,
                    TrangThai = true
                };
                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear();
                if (await _context.TaiKhoans.AnyAsync(account => account.TenDangNhap == model.TenDangNhap))
                {
                    ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                throw;
            }

            TempData["Success"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
            return RedirectToAction(nameof(Login));
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}