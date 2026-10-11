// Họ và tên: [Đào Minh Long]
// Mã sinh viên: [23203100068]`
// Nội dung: Module 1 - Quản lý Loại sản phẩm
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Data;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Controllers
{
    public class LoaiSanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaiSanPhamController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin() => HttpContext.Session.GetString("VaiTro") == "Admin";

        // GET: LoaiSanPham
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var list = await _context.LoaiSanPhams.ToListAsync();
            return View(list);
        }

        // GET: LoaiSanPham/Create
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: LoaiSanPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoaiSanPham loai)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                if (await _context.LoaiSanPhams.AnyAsync(l => l.TenLoai == loai.TenLoai))
                {
                    ModelState.AddModelError("TenLoai", "Tên loại sản phẩm đã tồn tại.");
                    return View(loai);
                }

                _context.Add(loai);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm loại sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(loai);
        }

        // GET: LoaiSanPham/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin() || id == null) return RedirectToAction("Login", "Account");

            var loai = await _context.LoaiSanPhams.FindAsync(id);
            if (loai == null) return NotFound();

            return View(loai);
        }

        // POST: LoaiSanPham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoaiSanPham loai)
        {
            if (!IsAdmin() || id != loai.MaLoai) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loai);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật loại sản phẩm thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.LoaiSanPhams.Any(e => e.MaLoai == loai.MaLoai))
                        return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(loai);
        }
    }
}