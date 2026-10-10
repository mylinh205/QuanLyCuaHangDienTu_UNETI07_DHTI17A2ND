using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Data;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
                return RedirectToAction("Login", "Account");

            ViewBag.ProductCount = await _context.SanPhams.CountAsync();
            ViewBag.CategoryCount = await _context.LoaiSanPhams.CountAsync();
            ViewBag.AccountCount = await _context.TaiKhoans.CountAsync();
            ViewBag.CustomerCount = await _context.KhachHangs.CountAsync();

            return View();
        }

        public async Task<IActionResult> Customers()
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
                return RedirectToAction("Login", "Account");

            var customers = await _context.KhachHangs
                .Include(customer => customer.TaiKhoan)
                .OrderBy(customer => customer.HoTen)
                .ToListAsync();

            return View(customers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCustomerStatus(int id)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
                return RedirectToAction("Login", "Account");

            var customer = await _context.KhachHangs
                .Include(item => item.TaiKhoan)
                .FirstOrDefaultAsync(item => item.MaKhachHang == id);

            if (customer == null) return NotFound();

            customer.TrangThai = !customer.TrangThai;
            if (customer.TaiKhoan != null)
                customer.TaiKhoan.TrangThai = customer.TrangThai;

            await _context.SaveChangesAsync();
            TempData["Success"] = customer.TrangThai
                ? "Đã mở khóa tài khoản khách hàng."
                : "Đã khóa tài khoản khách hàng.";

            return RedirectToAction(nameof(Customers));
        }
    }
}