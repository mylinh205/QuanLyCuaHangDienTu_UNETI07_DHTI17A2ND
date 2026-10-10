using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<SanPham> SanPhams => Set<SanPham>();
        public DbSet<LoaiSanPham> LoaiSanPhams => Set<LoaiSanPham>();
        public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();
        public DbSet<KhachHang> KhachHangs => Set<KhachHang>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaiKhoan>()
                .HasOne(t => t.KhachHang)
                .WithOne(k => k.TaiKhoan)
                .HasForeignKey<KhachHang>(k => k.MaTaiKhoan);

            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique();
        }
    }
}
