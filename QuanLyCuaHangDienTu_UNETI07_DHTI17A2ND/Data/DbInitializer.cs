using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            context.Database.ExecuteSqlRaw("""
                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = N'IX_TaiKhoans_TenDangNhap'
                      AND object_id = OBJECT_ID(N'dbo.TaiKhoans'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_TaiKhoans_TenDangNhap]
                    ON [dbo].[TaiKhoans] ([TenDangNhap]);
                END
                """);

            if (!context.TaiKhoans.Any(account => account.TenDangNhap == "admin"))
            {
                var admin = new TaiKhoan
                {
                    TenDangNhap = "admin",
                    HoTen = "Quản trị viên",
                    Email = "admin@uneti.local",
                    VaiTro = "Admin",
                    TrangThai = true
                };
                admin.MatKhau = new PasswordHasher<TaiKhoan>().HashPassword(admin, "Admin@123");
                context.TaiKhoans.Add(admin);
                context.SaveChanges();
            }

            var productGroups = new[]
            {
                new ProductGroup("Điện thoại", "Samsung", 4500000m, 1800000m, 12, new[]
                {
                    "Galaxy A15 128GB", "Galaxy A25 5G 128GB", "Galaxy A35 5G 128GB", "Galaxy A55 5G 256GB", "Galaxy S23 FE 128GB",
                    "Galaxy S24 256GB", "Galaxy S24+ 256GB", "Galaxy S24 Ultra 256GB", "Galaxy Z Flip5 256GB", "Galaxy Z Fold5 512GB"
                }),
                new ProductGroup("Laptop", "ASUS", 12000000m, 2500000m, 24, new[]
                {
                    "Vivobook Go 14 E1404", "Vivobook 15 X1504", "Vivobook S 14 OLED", "Zenbook 14 OLED UX3405", "Zenbook 14X OLED",
                    "TUF Gaming A15 FA507", "TUF Gaming F15 FX507", "ROG Zephyrus G14", "ROG Strix G16", "ExpertBook B1 B1502"
                }),
                new ProductGroup("Máy tính bảng", "Apple", 6500000m, 2000000m, 12, new[]
                {
                    "iPad 9 Wi-Fi 64GB", "iPad 10 Wi-Fi 64GB", "iPad 10 Wi-Fi 256GB", "iPad mini 6 Wi-Fi 64GB", "iPad mini 6 Wi-Fi 256GB",
                    "iPad Air 5 Wi-Fi 64GB", "iPad Air 5 Wi-Fi 256GB", "iPad Air 11 inch M2 128GB", "iPad Pro 11 inch M4 256GB", "iPad Pro 13 inch M4 512GB"
                }),
                new ProductGroup("Tivi", "LG", 7000000m, 3000000m, 24, new[]
                {
                    "Smart TV 4K 43 inch UR7550", "Smart TV 4K 50 inch UR7550", "Smart TV 4K 55 inch UR7550", "QNED 4K 55 inch QNED80", "QNED 4K 65 inch QNED80",
                    "OLED 4K 48 inch C3", "OLED 4K 55 inch C3", "OLED 4K 65 inch C3", "OLED evo 65 inch G3", "OLED evo 77 inch G3"
                }),
                new ProductGroup("Tai nghe", "Sony", 800000m, 500000m, 12, new[]
                {
                    "WH-CH520 Bluetooth", "WH-CH720N chống ồn", "WH-1000XM4 chống ồn", "WH-1000XM5 chống ồn", "WF-C500 True Wireless",
                    "WF-C700N chống ồn", "WF-1000XM4 True Wireless", "WF-1000XM5 True Wireless", "INZONE H3 Gaming", "INZONE Buds Gaming"
                }),
                new ProductGroup("Loa Bluetooth", "JBL", 500000m, 350000m, 12, new[]
                {
                    "Go 3 Bluetooth", "Go 4 Bluetooth", "Clip 4 Bluetooth", "Clip 5 Bluetooth", "Flip 6 Bluetooth",
                    "Flip Essential 2 Bluetooth", "Charge 5 Bluetooth", "Charge 5 Wi-Fi", "Xtreme 3 Bluetooth", "Boombox 3 Bluetooth"
                }),
                new ProductGroup("Đồng hồ thông minh", "Xiaomi", 1400000m, 600000m, 12, new[]
                {
                    "Smart Band 8", "Smart Band 8 Pro", "Smart Band 9", "Redmi Watch 3 Active", "Redmi Watch 4",
                    "Watch S1 Active", "Watch S3", "Watch 2 Lite", "Mi Watch", "Smart Band 7 Pro"
                }),
                new ProductGroup("Máy ảnh", "Canon", 10000000m, 4000000m, 24, new[]
                {
                    "EOS 2000D kèm lens 18-55mm", "EOS 250D kèm lens 18-55mm", "EOS 850D kèm lens 18-55mm", "EOS R50 kèm lens 18-45mm", "EOS R10 kèm lens 18-45mm",
                    "EOS R7 Body", "EOS R8 Body", "EOS R6 Mark II Body", "PowerShot G7 X Mark III", "PowerShot SX740 HS"
                }),
                new ProductGroup("Thiết bị mạng", "TP-Link", 300000m, 150000m, 12, new[]
                {
                    "Router Wi-Fi 4 TL-WR840N", "Router Wi-Fi 5 Archer C6", "Router Wi-Fi 5 Archer A8", "Router Wi-Fi 6 Archer AX10", "Router Wi-Fi 6 Archer AX23",
                    "Router Wi-Fi 6 Archer AX55", "Router Wi-Fi 6 Archer AX73", "Mesh Wi-Fi Deco E4 bộ 2", "Mesh Wi-Fi Deco X20 bộ 2", "USB Wi-Fi Adapter Archer T3U"
                }),
                new ProductGroup("Máy chơi game", "Nintendo", 3500000m, 900000m, 12, new[]
                {
                    "Nintendo Switch Lite Turquoise", "Nintendo Switch Lite Coral", "Nintendo Switch Lite Yellow", "Nintendo Switch Lite Blue", "Nintendo Switch V2 Neon",
                    "Nintendo Switch V2 Gray", "Nintendo Switch OLED White", "Nintendo Switch OLED Neon", "Nintendo Switch OLED Zelda Edition", "Nintendo Switch OLED Mario Edition"
                })
            };

            var categories = context.LoaiSanPhams.ToList();
            foreach (var group in productGroups)
            {
                if (categories.All(category => category.TenLoai != group.Category))
                {
                    var category = new LoaiSanPham { TenLoai = group.Category, TrangThai = true };
                    context.LoaiSanPhams.Add(category);
                    categories.Add(category);
                }
            }

            context.SaveChanges();

            if (context.SanPhams.Any())
                return;

            var categoryByName = categories.ToDictionary(category => category.TenLoai);
            var products = new List<SanPham>();
            var productIndex = 0;

            foreach (var group in productGroups)
            {
                for (var itemIndex = 0; itemIndex < group.Names.Length; itemIndex++)
                {
                    var status = productIndex is 23 or 57 or 88 or 96
                        ? "Tạm ngừng"
                        : productIndex is 49 or 79
                            ? "Ngừng kinh doanh"
                            : "Đang kinh doanh";

                    products.Add(new SanPham
                    {
                        TenSanPham = group.Names[itemIndex],
                        MaLoai = categoryByName[group.Category].MaLoai,
                        ThuongHieu = group.Brand,
                        DonGia = group.StartingPrice + group.PriceStep * itemIndex,
                        SoLuongTon = 5 + (productIndex * 7 % 36),
                        ThoiGianBaoHanh = group.WarrantyMonths,
                        MoTa = $"{group.Category} {group.Brand} chính hãng, bảo hành {group.WarrantyMonths} tháng.",
                        TrangThai = status
                    });
                    productIndex++;
                }
            }

            context.SanPhams.AddRange(products);
            context.SaveChanges();
        }

        private sealed record ProductGroup(
            string Category,
            string Brand,
            decimal StartingPrice,
            decimal PriceStep,
            int WarrantyMonths,
            string[] Names);
    }
}