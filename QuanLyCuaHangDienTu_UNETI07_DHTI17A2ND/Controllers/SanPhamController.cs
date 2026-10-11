// Họ và tên: [Nguyễn Văn Dũng ]
// Mã sinh viên: [23203100024]
// Nội dung thực hiện: Module 2 - Quản lý sản phẩm, Tìm kiếm, Lọc, Sắp xếp và Phân trang dữ liệu
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Data;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.ViewModels;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SanPhamController(ApplicationDbContext context)
        {
            _context = context;
        }
        private bool IsAdmin() => HttpContext.Session.GetString("VaiTro") == "Admin";

        private async Task LoadCategoryOptions(int? selectedId, bool activeOnly = true)
        {
            var categories = _context.LoaiSanPhams.AsQueryable();
            if (activeOnly)
                categories = categories.Where(category => category.TrangThai);

            ViewBag.MaLoai = new SelectList(
                await categories.OrderBy(category => category.TenLoai).ToListAsync(),
                "MaLoai",
                "TenLoai",
                selectedId);
        }

        private void ValidateProductStatus(string status)
        {
            if (status is not ("Đang kinh doanh" or "Tạm ngừng" or "Ngừng kinh doanh"))
                ModelState.AddModelError(nameof(SanPham.TrangThai), "Vui lòng chọn trạng thái hợp lệ.");
        }

        // GET: SanPham (Xem danh sách / Tra cứu)
        public async Task<IActionResult> Index(SanPhamIndexViewModel model)
        {
            if (model.PageSize is not (6 or 12 or 24))
                model.PageSize = 6;

            model.PageIndex = Math.Max(1, model.PageIndex);
            if (model.GiaTu.HasValue && model.GiaDen.HasValue && model.GiaTu > model.GiaDen)
                ModelState.AddModelError(nameof(model.GiaDen), "Giá tối đa phải lớn hơn hoặc bằng giá tối thiểu.");

            var query = _context.SanPhams.Include(s => s.LoaiSanPham).AsQueryable();

            // 1. TÌM KIẾM (Theo tên sản phẩm và thương hiệu bằng LINQ)
            if (!string.IsNullOrWhiteSpace(model.SearchTerm))
            {
                string kw = model.SearchTerm.Trim();
                query = query.Where(s => s.TenSanPham.Contains(kw) || s.ThuongHieu.Contains(kw));
            }

            // 2. LỌC (Kết hợp các điều kiện)
            if (model.MaLoai.HasValue && model.MaLoai > 0)
                query = query.Where(s => s.MaLoai == model.MaLoai.Value);

            if (!string.IsNullOrWhiteSpace(model.ThuongHieu))
                query = query.Where(s => s.ThuongHieu == model.ThuongHieu);

            if (!string.IsNullOrWhiteSpace(model.TrangThai))
                query = query.Where(s => s.TrangThai == model.TrangThai);

            if (model.GiaTu.HasValue && (!model.GiaDen.HasValue || model.GiaTu <= model.GiaDen))
                query = query.Where(s => s.DonGia >= model.GiaTu.Value);

            if (model.GiaDen.HasValue && (!model.GiaTu.HasValue || model.GiaTu <= model.GiaDen))
                query = query.Where(s => s.DonGia <= model.GiaDen.Value);

            // 3. SẮP XẾP
            IOrderedQueryable<SanPham> orderedQuery = model.SortOrder switch
            {
                "name_desc" => query.OrderByDescending(s => s.TenSanPham),
                "price_asc" => query.OrderBy(s => s.DonGia),
                "price_desc" => query.OrderByDescending(s => s.DonGia),
                "stock_asc" => query.OrderBy(s => s.SoLuongTon),
                "stock_desc" => query.OrderByDescending(s => s.SoLuongTon),
                "name_asc" => query.OrderBy(s => s.TenSanPham),
                _ => query.OrderBy(s => s.TenSanPham)
            };

            // 4. PHÂN TRANG (Truy vấn trực tiếp trên Server bằng Skip và Take)
            model.TotalItems = await orderedQuery.CountAsync();
            model.TotalPages = (int)Math.Ceiling(model.TotalItems / (double)model.PageSize);
            if (model.TotalPages < 1) model.TotalPages = 1;
            model.PageIndex = Math.Max(1, Math.Min(model.PageIndex, model.TotalPages));

            model.SanPhams = await orderedQuery
                .ThenBy(s => s.MaSanPham)
                .Skip((model.PageIndex - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            // Chuẩn bị dữ liệu Dropdown cho View
            ViewBag.MaLoaiList = new SelectList(await _context.LoaiSanPhams.Where(l => l.TrangThai).ToListAsync(), "MaLoai", "TenLoai", model.MaLoai);
            ViewBag.ThuongHieuList = await _context.SanPhams
                .Where(s => s.ThuongHieu != null && s.ThuongHieu != "")
                .Select(s => s.ThuongHieu)
                .Distinct()
                .OrderBy(brand => brand)
                .ToListAsync();
            ViewBag.IsAdmin = IsAdmin();

            return View(model);
        }

        // GET: SanPham/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sanPham = await _context.SanPhams
                .Include(s => s.LoaiSanPham)
                .FirstOrDefaultAsync(m => m.MaSanPham == id);

            if (sanPham == null) return NotFound();

            ViewBag.IsAdmin = IsAdmin();
            return View(sanPham);
        }

        // GET: SanPham/Create (Chỉ dành cho Admin)
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            await LoadCategoryOptions(null);
            return View(new SanPham());
        }

        // POST: SanPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sanPham)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            ValidateProductStatus(sanPham.TrangThai);
            if (!await _context.LoaiSanPhams.AnyAsync(category => category.MaLoai == sanPham.MaLoai && category.TrangThai))
                ModelState.AddModelError(nameof(sanPham.MaLoai), "Vui lòng chọn loại sản phẩm đang hoạt động.");

            if (ModelState.IsValid)
            {
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm mới sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }

            await LoadCategoryOptions(sanPham.MaLoai);
            return View(sanPham);
        }

        // GET: SanPham/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (id == null) return NotFound();

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null) return NotFound();

            await LoadCategoryOptions(sanPham.MaLoai, activeOnly: false);
            return View(sanPham);
        }

        // POST: SanPham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SanPham sanPham)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (id != sanPham.MaSanPham) return NotFound();

            ValidateProductStatus(sanPham.TrangThai);
            if (!await _context.LoaiSanPhams.AnyAsync(category => category.MaLoai == sanPham.MaLoai))
                ModelState.AddModelError(nameof(sanPham.MaLoai), "Vui lòng chọn loại sản phẩm hợp lệ.");

            var existingProduct = await _context.SanPhams.FindAsync(id);
            if (existingProduct == null) return NotFound();

            if (ModelState.IsValid)
            {
                existingProduct.TenSanPham = sanPham.TenSanPham;
                existingProduct.MaLoai = sanPham.MaLoai;
                existingProduct.ThuongHieu = sanPham.ThuongHieu;
                existingProduct.XuatXu = sanPham.XuatXu;
                existingProduct.DonGia = sanPham.DonGia;
                existingProduct.SoLuongTon = sanPham.SoLuongTon;
                existingProduct.ThoiGianBaoHanh = sanPham.ThoiGianBaoHanh;
                existingProduct.MoTa = sanPham.MoTa;
                existingProduct.TrangThai = sanPham.TrangThai;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }

            await LoadCategoryOptions(sanPham.MaLoai, activeOnly: false);
            return View(sanPham);
        }

        // GET: SanPham/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (id == null) return NotFound();

            var sanPham = await _context.SanPhams
                .Include(product => product.LoaiSanPham)
                .FirstOrDefaultAsync(product => product.MaSanPham == id);

            return sanPham == null ? NotFound() : View(sanPham);
        }

        // POST: SanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null) return NotFound();

            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: SanPham/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (status is not ("Đang kinh doanh" or "Tạm ngừng" or "Ngừng kinh doanh"))
                return BadRequest("Trạng thái sản phẩm không hợp lệ.");

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null) return NotFound();

            sanPham.TrangThai = status;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật trạng thái sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}