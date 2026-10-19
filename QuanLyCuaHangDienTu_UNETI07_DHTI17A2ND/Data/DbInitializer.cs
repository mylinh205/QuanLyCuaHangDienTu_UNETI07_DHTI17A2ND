using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Chỉ dùng EnsureCreated nếu không dùng Migrations.
            context.Database.EnsureCreated();
            context.Database.ExecuteSqlRaw("IF COL_LENGTH('SanPhams', 'HinhAnh') IS NULL ALTER TABLE SanPhams ADD HinhAnh NVARCHAR(500) NULL;");

            SeedAdmin(context);
            SeedProducts(context);
            SeedModule3(context);
        }

        // =====================================
        // 1. TẠO TÀI KHOẢN ADMIN
        // =====================================
        private static void SeedAdmin(ApplicationDbContext context)
        {
            if (context.TaiKhoans.Any(
                x => x.TenDangNhap == "admin"))
                return;

            var admin = new TaiKhoan
            {
                TenDangNhap = "admin",
                HoTen = "Quản trị viên",
                Email = "admin@uneti.local",
                VaiTro = "Admin",
                TrangThai = true
            };

            admin.MatKhau = new PasswordHasher<TaiKhoan>()
                .HashPassword(admin, "Admin@123");

            context.TaiKhoans.Add(admin);
            context.SaveChanges();
        }

        // =====================================
        // 2. TẠO DANH MỤC VÀ 100 SẢN PHẨM
        // =====================================
        private static void SeedProducts(ApplicationDbContext context)
        {
            var productGroups = new[]
            {
                new ProductGroup(
                    "Điện thoại", "Samsung",
                    4500000m, 1800000m, 12,
                    new[]
                    {
                        "Galaxy A15 128GB",
                        "Galaxy A25 5G 128GB",
                        "Galaxy A35 5G 128GB",
                        "Galaxy A55 5G 256GB",
                        "Galaxy S23 FE 128GB",
                        "Galaxy S24 256GB",
                        "Galaxy S24+ 256GB",
                        "Galaxy S24 Ultra 256GB",
                        "Galaxy Z Flip5 256GB",
                        "Galaxy Z Fold5 512GB"
                    }),

                new ProductGroup(
                    "Laptop", "ASUS",
                    12000000m, 2500000m, 24,
                    new[]
                    {
                        "Vivobook Go 14 E1404",
                        "Vivobook 15 X1504",
                        "Vivobook S 14 OLED",
                        "Zenbook 14 OLED UX3405",
                        "Zenbook 14X OLED",
                        "TUF Gaming A15 FA507",
                        "TUF Gaming F15 FX507",
                        "ROG Zephyrus G14",
                        "ROG Strix G16",
                        "ExpertBook B1 B1502"
                    }),

                new ProductGroup(
                    "Máy tính bảng", "Apple",
                    6500000m, 2000000m, 12,
                    new[]
                    {
                        "iPad 9 Wi-Fi 64GB",
                        "iPad 10 Wi-Fi 64GB",
                        "iPad 10 Wi-Fi 256GB",
                        "iPad mini 6 Wi-Fi 64GB",
                        "iPad mini 6 Wi-Fi 256GB",
                        "iPad Air 5 Wi-Fi 64GB",
                        "iPad Air 5 Wi-Fi 256GB",
                        "iPad Air 11 inch M2 128GB",
                        "iPad Pro 11 inch M4 256GB",
                        "iPad Pro 13 inch M4 512GB"
                    }),

                new ProductGroup(
                    "Tivi", "LG",
                    7000000m, 3000000m, 24,
                    new[]
                    {
                        "Smart TV 4K 43 inch UR7550",
                        "Smart TV 4K 50 inch UR7550",
                        "Smart TV 4K 55 inch UR7550",
                        "QNED 4K 55 inch QNED80",
                        "QNED 4K 65 inch QNED80",
                        "OLED 4K 48 inch C3",
                        "OLED 4K 55 inch C3",
                        "OLED 4K 65 inch C3",
                        "OLED evo 65 inch G3",
                        "OLED evo 77 inch G3"
                    }),

                new ProductGroup(
                    "Tai nghe", "Sony",
                    800000m, 500000m, 12,
                    new[]
                    {
                        "WH-CH520 Bluetooth",
                        "WH-CH720N chống ồn",
                        "WH-1000XM4 chống ồn",
                        "WH-1000XM5 chống ồn",
                        "WF-C500 True Wireless",
                        "WF-C700N chống ồn",
                        "WF-1000XM4 True Wireless",
                        "WF-1000XM5 True Wireless",
                        "INZONE H3 Gaming",
                        "INZONE Buds Gaming"
                    }),

                new ProductGroup(
                    "Loa Bluetooth", "JBL",
                    500000m, 350000m, 12,
                    new[]
                    {
                        "Go 3 Bluetooth",
                        "Go 4 Bluetooth",
                        "Clip 4 Bluetooth",
                        "Clip 5 Bluetooth",
                        "Flip 6 Bluetooth",
                        "Flip Essential 2 Bluetooth",
                        "Charge 5 Bluetooth",
                        "Charge 5 Wi-Fi",
                        "Xtreme 3 Bluetooth",
                        "Boombox 3 Bluetooth"
                    }),

                new ProductGroup(
                    "Đồng hồ thông minh", "Xiaomi",
                    1400000m, 600000m, 12,
                    new[]
                    {
                        "Smart Band 8",
                        "Smart Band 8 Pro",
                        "Smart Band 9",
                        "Redmi Watch 3 Active",
                        "Redmi Watch 4",
                        "Watch S1 Active",
                        "Watch S3",
                        "Watch 2 Lite",
                        "Mi Watch",
                        "Smart Band 7 Pro"
                    }),

                new ProductGroup(
                    "Máy ảnh", "Canon",
                    10000000m, 4000000m, 24,
                    new[]
                    {
                        "EOS 2000D kèm lens 18-55mm",
                        "EOS 250D kèm lens 18-55mm",
                        "EOS 850D kèm lens 18-55mm",
                        "EOS R50 kèm lens 18-45mm",
                        "EOS R10 kèm lens 18-45mm",
                        "EOS R7 Body",
                        "EOS R8 Body",
                        "EOS R6 Mark II Body",
                        "PowerShot G7 X Mark III",
                        "PowerShot SX740 HS"
                    }),

                new ProductGroup(
                    "Thiết bị mạng", "TP-Link",
                    300000m, 150000m, 12,
                    new[]
                    {
                        "Router Wi-Fi 4 TL-WR840N",
                        "Router Wi-Fi 5 Archer C6",
                        "Router Wi-Fi 5 Archer A8",
                        "Router Wi-Fi 6 Archer AX10",
                        "Router Wi-Fi 6 Archer AX23",
                        "Router Wi-Fi 6 Archer AX55",
                        "Router Wi-Fi 6 Archer AX73",
                        "Mesh Wi-Fi Deco E4 bộ 2",
                        "Mesh Wi-Fi Deco X20 bộ 2",
                        "USB Wi-Fi Adapter Archer T3U"
                    }),

                new ProductGroup(
                    "Máy chơi game", "Nintendo",
                    3500000m, 900000m, 12,
                    new[]
                    {
                        "Nintendo Switch Lite Turquoise",
                        "Nintendo Switch Lite Coral",
                        "Nintendo Switch Lite Yellow",
                        "Nintendo Switch Lite Blue",
                        "Nintendo Switch V2 Neon",
                        "Nintendo Switch V2 Gray",
                        "Nintendo Switch OLED White",
                        "Nintendo Switch OLED Neon",
                        "Nintendo Switch OLED Zelda Edition",
                        "Nintendo Switch OLED Mario Edition"
                    })
            };

            var categories = context.LoaiSanPhams.ToList();

            foreach (var group in productGroups)
            {
                if (categories.Any(x => x.TenLoai == group.Category))
                    continue;

                var category = new LoaiSanPham
                {
                    TenLoai = group.Category,
                    TrangThai = true
                };

                context.LoaiSanPhams.Add(category);
                categories.Add(category);
            }

            context.SaveChanges();

            // =====================================
            // 4. ĐỒNG BỘ SẢN PHẨM MẪU
            // =====================================
            var categoryByName = categories.ToDictionary(x => x.TenLoai);
            var existingProducts = context.SanPhams.ToList();
            int productIndex = 0;

            foreach (var group in productGroups)
            {
                for (int i = 0; i < group.Names.Length; i++)
                {
                    string name = group.Names[i];
                    int categoryId = categoryByName[group.Category].MaLoai;
                    string status = productIndex == 23 || productIndex == 57 || productIndex == 88 || productIndex == 96
                        ? "Tạm ngừng"
                        : productIndex == 49 || productIndex == 79
                            ? "Ngừng kinh doanh"
                            : "Đang kinh doanh";

                    var product = existingProducts.FirstOrDefault(
                        x => x.MaLoai == categoryId && x.TenSanPham == name);

                    if (product == null)
                    {
                        product = new SanPham
                        {
                            TenSanPham = name,
                            MaLoai = categoryId,
                            SoLuongTon = 5 + (productIndex * 7 % 36),
                            HinhAnh = GetProductImageUrl(group.Brand, name)
                        };
                        context.SanPhams.Add(product);
                        existingProducts.Add(product);
                    }

                    product.ThuongHieu = group.Brand;
                    product.DonGia = group.StartingPrice + group.PriceStep * i;
                    product.ThoiGianBaoHanh = group.WarrantyMonths;
                    product.MoTa = $"{group.Category} {group.Brand} chính hãng, bảo hành {group.WarrantyMonths} tháng.";
                    product.TrangThai = status;
                    productIndex++;
                }
            }

            context.SaveChanges();

        private static string GetProductImageUrl(string brand, string productName)
        {
            var searchTerms = Uri.EscapeDataString($"{brand} {productName}");
            return $"https://tse1.mm.bing.net/th?q={searchTerms}";
        }

        // =====================================
        // 3. MODULE 3 - QUẢN LÝ KHÁCH HÀNG
        // =====================================
        private static void SeedModule3(ApplicationDbContext context)
        {
            // Dữ liệu dùng cho học tập/thử nghiệm.
            // Email và tên đăng nhập được thay bằng dữ liệu demo.
            var customers = new[]
            {
                new CustomerSeed(
                    "khachhang_demo01",
                    "Phạm Thành",
                    "thanh.demo@example.com"),

                new CustomerSeed(
                    "khachhang_demo02",
                    "Đào Minh Long",
                    "long.demo@example.com"),

                new CustomerSeed(
                    "khachhang_demo1",
                    "Khách hàng mẫu 1",
                    "khachhang1@example.com"),

                new CustomerSeed(
                    "khachhang_demo2",
                    "Khách hàng mẫu 2",
                    "khachhang2@example.com"),

                new CustomerSeed(
                    "khachhang_demo3",
                    "Nguyễn Văn Dũng",
                    "dung.demo@example.com"),

                new CustomerSeed(
                    "khachhang_demo4",
                    "Vũ Thành Đạt",
                    "dat.demo@example.com"),

                new CustomerSeed(
                    "khachhang_demo5",
                    "Nguyễn Văn An",
                    "an.demo@example.com")
            };

            var hasher = new PasswordHasher<TaiKhoan>();

            // =====================================
            // 3.1. TẠO TÀI KHOẢN VÀ KHÁCH HÀNG
            // =====================================
            foreach (var item in customers)
            {
                // Ưu tiên tìm khách hàng đã tồn tại
                // theo email demo hoặc họ tên.
                // Kiểm tra họ tên chỉ phù hợp với bộ
                // dữ liệu mẫu này, không dùng làm khóa
                // định danh trong hệ thống thực tế.
                var existingCustomer = context.KhachHangs
                    .FirstOrDefault(x =>
                        x.Email == item.Email ||
                        x.HoTen == item.Name);

                // Nếu khách hàng đã tồn tại,
                // giữ nguyên dữ liệu và tài khoản cũ.
                if (existingCustomer != null)
                    continue;

                var account = context.TaiKhoans
                    .FirstOrDefault(x =>
                        x.TenDangNhap == item.Username);

                if (account == null)
                {
                    account = new TaiKhoan
                    {
                        TenDangNhap = item.Username,
                        HoTen = item.Name,
                        Email = item.Email,
                        VaiTro = "KhachHang",
                        TrangThai = true
                    };

                    account.MatKhau = hasher.HashPassword(
                        account, "Demo@12345");

                    context.TaiKhoans.Add(account);
                    context.SaveChanges();
                }

                // Không tạo khách hàng thứ hai cho
                // cùng một tài khoản.
                if (context.KhachHangs.Any(
                    x => x.MaTaiKhoan == account.MaTaiKhoan))
                    continue;

                context.KhachHangs.Add(new KhachHang
                {
                    MaTaiKhoan = account.MaTaiKhoan,
                    HoTen = item.Name,
                    Email = item.Email,
                    NgayDangKy = DateTime.Now,
                    TrangThai = true
                });

                context.SaveChanges();
            }

            // =====================================
            // 3.2. TẠO 2 ĐƠN HÀNG VÀ CHI TIẾT
            // =====================================
            var demoOrders = new[]
            {
                new OrderSeed(
                    "Khách hàng mẫu 1",
                    "WH-1000XM5 chống ồn",
                    "Địa chỉ giao hàng mẫu 1"),

                new OrderSeed(
                    "Khách hàng mẫu 2",
                    "WF-C700N chống ồn",
                    "Địa chỉ giao hàng mẫu 2")
            };

            foreach (var item in demoOrders)
            {
                var customer = context.KhachHangs
                    .FirstOrDefault(x => x.HoTen == item.CustomerName);

                if (customer == null)
                    continue;

                var product = context.SanPhams
                    .FirstOrDefault(x =>
                        x.TenSanPham == item.ProductName);

                if (product == null)
                    continue;

                // Không tạo đơn demo trùng.
                bool orderExists = context.DonHangs.Any(x =>
                    x.MaKhachHang == customer.MaKhachHang &&
                    x.DiaChiGiaoHang == item.Address);

                if (orderExists)
                    continue;

                decimal price = product.DonGia;

                var order = new DonHang
                {
                    MaKhachHang = customer.MaKhachHang,
                    NgayDat = DateTime.Now,
                    DiaChiGiaoHang = item.Address,
                    TongTien = price,
                    TrangThai = "Chờ xác nhận",

                    ChiTietDonHangs = new List<ChiTietDonHang>
                    {
                        new ChiTietDonHang
                        {
                            MaSanPham = product.MaSanPham,
                            SoLuong = 1,
                            DonGia = price,
                            ThanhTien = price
                        }
                    }
                };

                context.DonHangs.Add(order);
                context.SaveChanges();
            }
        }

        // =====================================
        // 4. CÁC LỚP LƯU DỮ LIỆU MẪU
        // =====================================
        private sealed record ProductGroup(
            string Category,
            string Brand,
            decimal StartingPrice,
            decimal PriceStep,
            int WarrantyMonths,
            string[] Names);

        private sealed record CustomerSeed(
            string Username,
            string Name,
            string Email);

        private sealed record OrderSeed(
            string CustomerName,
            string ProductName,
            string Address);
    }
}