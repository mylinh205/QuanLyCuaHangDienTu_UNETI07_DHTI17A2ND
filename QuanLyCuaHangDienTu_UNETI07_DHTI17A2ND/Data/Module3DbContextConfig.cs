using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models;
namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Data;
// GỌI Module3DbContextConfig.Configure(modelBuilder) trong OnModelCreating của DbContext CHUNG.
public static class Module3DbContextConfig
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KhachHang>().HasIndex(x => x.MaTaiKhoan).IsUnique();
        modelBuilder.Entity<DonHang>().HasOne(x => x.KhachHang).WithMany(x => x.DonHangs)
            .HasForeignKey(x => x.MaKhachHang).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ChiTietDonHang>().HasKey(x => new { x.MaDonHang, x.MaSanPham });
        modelBuilder.Entity<ChiTietDonHang>().HasOne(x => x.DonHang).WithMany(x => x.ChiTietDonHangs)
            .HasForeignKey(x => x.MaDonHang).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ChiTietDonHang>().HasOne(x => x.SanPham).WithMany()
            .HasForeignKey(x => x.MaSanPham).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DonHang>().Property(x => x.TongTien).HasPrecision(18, 2);
        modelBuilder.Entity<ChiTietDonHang>().Property(x => x.DonGia).HasPrecision(18, 2);
        modelBuilder.Entity<ChiTietDonHang>().Property(x => x.ThanhTien).HasPrecision(18, 2);
    }
}
