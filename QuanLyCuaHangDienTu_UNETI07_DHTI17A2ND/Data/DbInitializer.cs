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
            // Chỉ dùng EnsureCreated khi dự án không sử dụng Migrations.
            // Nếu database đã tồn tại, lệnh này không cập nhật cấu trúc bảng.
            context.Database.EnsureCreated();
            context.Database.ExecuteSqlRaw("IF COL_LENGTH('SanPhams', 'HinhAnh') IS NULL ALTER TABLE SanPhams ADD HinhAnh NVARCHAR(500) NULL;");

            // =====================================
            // 1. TẠO TÀI KHOẢN ADMIN
            // =====================================
            if (!context.TaiKhoans.Any(x => x.TenDangNhap == "admin"))
            {
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
            // 2. DỮ LIỆU MẪU SẢN PHẨM
            // =====================================
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

            // =====================================
            // 3. TẠO LOẠI SẢN PHẨM
            // =====================================
            var categories = context.LoaiSanPhams.ToList();

            foreach (var group in productGroups)
            {
                bool exists = categories.Any(x => x.TenLoai == group.Category);

                if (!exists)
                {
                    var category = new LoaiSanPham
                    {
                        TenLoai = group.Category,
                        TrangThai = true
                    };

                    context.LoaiSanPhams.Add(category);
                    categories.Add(category);
                }
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

            var seededImageQuery = "?auto=format&fit=crop&w=1000&q=80";
            var productsWithCategoryImages = context.SanPhams
                .Where(product => product.HinhAnh == null || product.HinhAnh.Contains(seededImageQuery))
                .ToList();

            foreach (var product in productsWithCategoryImages)
                product.HinhAnh = GetProductImageUrl(product.ThuongHieu, product.TenSanPham);

            if (productsWithCategoryImages.Count > 0)
                context.SaveChanges();

            // =====================================
            // 5. TẠO KHÁCH HÀNG & ĐƠN HÀNG MẪU (Cho phần Thống kê / Sinh viên 4)
            // =====================================
            if (!context.DonHangs.Any())
            {
                // Tạo tài khoản khách hàng mẫu nếu chưa có
                var tkKhach = context.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == "khachhang01");
                if (tkKhach == null)
                {
                    tkKhach = new TaiKhoan
                    {
                        TenDangNhap = "khachhang01",
                        HoTen = "Nguyễn Văn Khách",
                        Email = "khachhang@uneti.local",
                        VaiTro = "KhachHang",
                        TrangThai = true
                    };
                    tkKhach.MatKhau = new PasswordHasher<TaiKhoan>().HashPassword(tkKhach, "123456");
                    context.TaiKhoans.Add(tkKhach);
                    context.SaveChanges();
                }

                // Tạo thông tin Khách hàng nếu chưa liên kết
                var khachHang = context.KhachHangs.FirstOrDefault(x => x.MaTaiKhoan == tkKhach.MaTaiKhoan);
                if (khachHang == null)
                {
                    khachHang = new KhachHang
                    {
                        MaTaiKhoan = tkKhach.MaTaiKhoan,
                        HoTen = tkKhach.HoTen,
                        SoDienThoai = "0987654321",
                        Email = tkKhach.Email,
                        DiaChi = "Hà Nội",
                        NgayDangKy = DateTime.Now.AddDays(-10)
                    };
                    context.KhachHangs.Add(khachHang);
                    context.SaveChanges();
                }

                // Lấy ra một vài sản phẩm có sẵn để làm chi tiết đơn hàng
                var listSanPham = context.SanPhams.Take(2).ToList();
                if (listSanPham.Count >= 2)
                {
                    var sp1 = listSanPham[0];
                    var sp2 = listSanPham[1];

                    // Đơn hàng 1: Đã hoàn thành (Dùng để test Thống kê doanh thu & Sản phẩm bán chạy)
                    var donHang1 = new DonHang
                    {
                        MaKhachHang = khachHang.MaKhachHang,
                        NgayDat = DateTime.Now.AddDays(-3),
                        DiaChiGiaoHang = khachHang.DiaChi ?? "Hà Nội",
                        TongTien = (sp1.DonGia * 1) + (sp2.DonGia * 2),
                        TrangThai = "Đã hoàn thành",
                        ChiTietDonHangs = new List<ChiTietDonHang>
                        {
                            new ChiTietDonHang
                            {
                                MaSanPham = sp1.MaSanPham,
                                SoLuong = 1,
                                DonGia = sp1.DonGia,
                                ThanhTien = sp1.DonGia * 1
                            },
                            new ChiTietDonHang
                            {
                                MaSanPham = sp2.MaSanPham,
                                SoLuong = 2,
                                DonGia = sp2.DonGia,
                                ThanhTien = sp2.DonGia * 2
                            }
                        }
                    };

                    // Đơn hàng 2: Chờ xác nhận (Dùng để test Quản lý danh sách đơn hàng)
                    var donHang2 = new DonHang
                    {
                        MaKhachHang = khachHang.MaKhachHang,
                        NgayDat = DateTime.Now.AddDays(-1),
                        DiaChiGiaoHang = khachHang.DiaChi ?? "Hà Nội",
                        TongTien = sp1.DonGia * 1,
                        TrangThai = "Chờ xác nhận",
                        ChiTietDonHangs = new List<ChiTietDonHang>
                        {
                            new ChiTietDonHang
                            {
                                MaSanPham = sp1.MaSanPham,
                                SoLuong = 1,
                                DonGia = sp1.DonGia,
                                ThanhTien = sp1.DonGia * 1
                            }
                        }
                    };

                    context.DonHangs.AddRange(donHang1, donHang2);
                    context.SaveChanges();
                }
            }

        }

        private static string GetProductImageUrl(string brand, string productName)
        {
            var searchTerms = Uri.EscapeDataString($"{brand} {productName}");
            return $"https://tse1.mm.bing.net/th?q={searchTerms}";
        }

        // =====================================
        // 6. LỚP LƯU DỮ LIỆU NHÓM
        // =====================================
        private sealed record ProductGroup(
            string Category,
            string Brand,
            decimal StartingPrice,
            decimal PriceStep,
            int WarrantyMonths,
            string[] Names);
    }
}