
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // MODULE 1: Tài khoản và loại sản phẩm
        public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();
        public DbSet<LoaiSanPham> LoaiSanPhams => Set<LoaiSanPham>();

        // MODULE 2: Quản lý sản phẩm
        public DbSet<SanPham> SanPhams => Set<SanPham>();

        // MODULE 3: Khách hàng và đặt hàng
        public DbSet<KhachHang> KhachHangs => Set<KhachHang>();
        public DbSet<DonHang> DonHangs => Set<DonHang>();
        public DbSet<ChiTietDonHang> ChiTietDonHangs
            => Set<ChiTietDonHang>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tên đăng nhập không được trùng
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique();

            // Tài khoản (1) - (1) Khách hàng
            modelBuilder.Entity<TaiKhoan>()
                .HasOne(t => t.KhachHang)
                .WithOne(k => k.TaiKhoan)
                .HasForeignKey<KhachHang>(k => k.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            // Khách hàng (1) - (N) Đơn hàng
            modelBuilder.Entity<DonHang>()
                .HasOne(d => d.KhachHang)
                .WithMany(k => k.DonHangs)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);

            // Khóa chính kép của ChiTietDonHang
            modelBuilder.Entity<ChiTietDonHang>()
                .HasKey(ct => new
                {
                    ct.MaDonHang,
                    ct.MaSanPham
                });

            // Đơn hàng (1) - (N) Chi tiết đơn hàng
            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(ct => ct.DonHang)
                .WithMany(d => d.ChiTietDonHangs)
                .HasForeignKey(ct => ct.MaDonHang)
                .OnDelete(DeleteBehavior.Cascade);

            // Sản phẩm (1) - (N) Chi tiết đơn hàng
            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(ct => ct.SanPham)
                .WithMany()
                .HasForeignKey(ct => ct.MaSanPham)
                .OnDelete(DeleteBehavior.Restrict);

            // Kiểu dữ liệu tiền tệ
            modelBuilder.Entity<DonHang>()
                .Property(d => d.TongTien)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ChiTietDonHang>()
                .Property(ct => ct.DonGia)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ChiTietDonHang>()
                .Property(ct => ct.ThanhTien)
                .HasColumnType("decimal(18,2)");
        }
    }
}
