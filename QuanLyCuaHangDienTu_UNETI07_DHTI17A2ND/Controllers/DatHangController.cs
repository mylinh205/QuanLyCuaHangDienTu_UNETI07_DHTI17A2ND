
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Data;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.ViewModels;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Controllers
{
    public class DatHangController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DatHangController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Lấy mã tài khoản đang đăng nhập
        private int? AccountId =>
            HttpContext.Session.GetInt32("MaTaiKhoan");

        // Kiểm tra quyền khách hàng
        private bool IsCustomer =>
            HttpContext.Session.GetString("VaiTro") == "KhachHang";

        // Lấy thông tin khách hàng hiện tại
        private async Task<KhachHang?> CurrentCustomer()
        {
            if (!AccountId.HasValue)
                return null;

            return await _db.KhachHangs
                .Include(x => x.TaiKhoan)
                .FirstOrDefaultAsync(
                    x => x.MaTaiKhoan == AccountId.Value);
        }

        // =====================================
        // 1. HIỂN THỊ FORM ĐẶT HÀNG
        // =====================================
        [HttpGet]
        public async Task<IActionResult> Create(int maSanPham)
        {
            if (AccountId == null)
                return RedirectToAction("Login", "Account");

            if (!IsCustomer)
                return Forbid();

            var kh = await CurrentCustomer();

            if (kh == null ||
                !kh.TrangThai ||
                kh.TaiKhoan == null ||
                !kh.TaiKhoan.TrangThai)
            {
                return BadRequest(
                    "Tài khoản hoặc hồ sơ khách hàng không hoạt động");
            }

            var sp = await _db.SanPhams
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.MaSanPham == maSanPham);

            if (sp == null)
                return NotFound("Sản phẩm không tồn tại");

            if (sp.TrangThai != "Đang kinh doanh")
                return BadRequest(
                    "Sản phẩm không đang kinh doanh");

            if (sp.SoLuongTon <= 0)
                return BadRequest(
                    "Sản phẩm đã hết hàng");

            if (sp.DonGia <= 0)
                return BadRequest(
                    "Đơn giá sản phẩm không hợp lệ");

            ViewBag.SanPham = sp;

            var vm = new DatHangVm
            {
                MaSanPham = maSanPham,
                SoLuong = 1,
                DiaChiGiaoHang = kh.DiaChi ?? ""
            };

            return View(vm);
        }

        // =====================================
        // 2. XỬ LÝ ĐẶT HÀNG
        // =====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DatHangVm vm)
        {
            if (AccountId == null)
                return RedirectToAction("Login", "Account");

            if (!IsCustomer)
                return Forbid();

            var kh = await CurrentCustomer();

            if (kh == null ||
                !kh.TrangThai ||
                kh.TaiKhoan == null ||
                !kh.TaiKhoan.TrangThai)
            {
                return BadRequest(
                    "Tài khoản hoặc hồ sơ khách hàng không hoạt động");
            }

            // Kiểm tra dữ liệu nhập
            if (vm.SoLuong <= 0)
            {
                ModelState.AddModelError(
                    nameof(vm.SoLuong),
                    "Số lượng đặt phải lớn hơn 0");
            }

            if (string.IsNullOrWhiteSpace(
                vm.DiaChiGiaoHang))
            {
                ModelState.AddModelError(
                    nameof(vm.DiaChiGiaoHang),
                    "Vui lòng nhập địa chỉ giao hàng");
            }

            if (!ModelState.IsValid)
                return await CreateInvalid(vm);

            // Bắt đầu giao dịch
            // Module 4 vẫn phải kiểm tra lại tồn kho
            // khi xác nhận đơn hàng.
            await using var tx =
                await _db.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var sp = await _db.SanPhams
                .FirstOrDefaultAsync(
                    x => x.MaSanPham == vm.MaSanPham);

            if (sp == null)
            {
                ModelState.AddModelError(
                    "",
                    "Sản phẩm không tồn tại");

                await tx.RollbackAsync();
                return await CreateInvalid(vm);
            }

            // Kiểm tra trạng thái sản phẩm
            if (sp.TrangThai != "Đang kinh doanh")
            {
                ModelState.AddModelError(
                    "",
                    "Sản phẩm không đang kinh doanh");
            }

            // Kiểm tra tồn kho
            if (sp.SoLuongTon < vm.SoLuong)
            {
                ModelState.AddModelError(
                    nameof(vm.SoLuong),
                    "Số lượng đặt vượt quá tồn kho");
            }

            // Kiểm tra đơn giá
            if (sp.DonGia <= 0)
            {
                ModelState.AddModelError(
                    "",
                    "Đơn giá sản phẩm không hợp lệ");
            }

            if (!ModelState.IsValid)
            {
                await tx.RollbackAsync();
                return await CreateInvalid(vm);
            }

            // Lấy giá từ database
            decimal donGia = sp.DonGia;

            // Tính thành tiền trên server
            decimal thanhTien = donGia * vm.SoLuong;

            // Tạo đơn hàng
            var donHang = new DonHang
            {
                MaKhachHang = kh.MaKhachHang,
                NgayDat = DateTime.Now,
                DiaChiGiaoHang =
                    vm.DiaChiGiaoHang.Trim(),
                TongTien = thanhTien,
                TrangThai = "Chờ xác nhận"
            };

            // Tạo chi tiết đơn hàng
            var chiTiet = new ChiTietDonHang
            {
                MaSanPham = sp.MaSanPham,
                SoLuong = vm.SoLuong,
                DonGia = donGia,
                ThanhTien = thanhTien
            };

            // Liên kết chi tiết với đơn hàng
            donHang.ChiTietDonHangs.Add(chiTiet);

            // Lưu vào database
            _db.DonHangs.Add(donHang);

            await _db.SaveChangesAsync();

            // Hoàn thành giao dịch
            await tx.CommitAsync();

            TempData["Success"] =
                "Đặt hàng thành công, đang chờ xác nhận";

            return RedirectToAction(
                nameof(Details),
                new { id = donHang.MaDonHang });
        }

        // =====================================
        // 3. HIỂN THỊ LẠI FORM KHI CÓ LỖI
        // =====================================
        private async Task<IActionResult> CreateInvalid(
            DatHangVm vm)
        {
            ViewBag.SanPham = await _db.SanPhams
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.MaSanPham == vm.MaSanPham);

            return View("Create", vm);
        }

        // =====================================
        // 4. DANH SÁCH ĐƠN HÀNG
        // =====================================
        [HttpGet]
        public async Task<IActionResult> Index(
            string? trangThai)
        {
            if (AccountId == null)
                return RedirectToAction("Login", "Account");

            if (!IsCustomer)
                return Forbid();

            var kh = await CurrentCustomer();

            if (kh == null)
                return NotFound(
                    "Không tìm thấy khách hàng");

            var query = _db.DonHangs
                .AsNoTracking()
                .Where(x =>
                    x.MaKhachHang == kh.MaKhachHang);

            // Lọc theo trạng thái đơn hàng
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(
                    x => x.TrangThai == trangThai);
            }

            ViewBag.TrangThai = trangThai;

            var danhSach = await query
                .OrderByDescending(x => x.NgayDat)
                .ToListAsync();

            return View(danhSach);
        }

        // =====================================
        // 5. XEM CHI TIẾT ĐƠN HÀNG
        // =====================================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (AccountId == null)
                return RedirectToAction("Login", "Account");

            if (!IsCustomer)
                return Forbid();

            var kh = await CurrentCustomer();

            if (kh == null)
                return NotFound();

            var donHang = await _db.DonHangs
                .AsNoTracking()
                .Include(x => x.ChiTietDonHangs)
                    .ThenInclude(x => x.SanPham)
                .FirstOrDefaultAsync(x =>
                    x.MaDonHang == id &&
                    x.MaKhachHang == kh.MaKhachHang);

            if (donHang == null)
            {
                return NotFound(
                    "Không tìm thấy đơn hàng");
            }

            return View(donHang);
        }

        // =====================================
        // 6. LỊCH SỬ MUA HÀNG
        // =====================================
        [HttpGet]
        public async Task<IActionResult> History()
        {
            if (AccountId == null)
                return RedirectToAction("Login", "Account");

            if (!IsCustomer)
                return Forbid();

            var kh = await CurrentCustomer();

            if (kh == null)
                return NotFound();

            var danhSach = await _db.DonHangs
                .AsNoTracking()
                .Where(x =>
                    x.MaKhachHang == kh.MaKhachHang &&
                    x.TrangThai == "Đã hoàn thành")
                .OrderByDescending(x => x.NgayDat)
                .ToListAsync();

            ViewBag.TrangThai = "Đã hoàn thành";

            // Sử dụng lại giao diện danh sách
            return View("Index", danhSach);
        }
    }
}
