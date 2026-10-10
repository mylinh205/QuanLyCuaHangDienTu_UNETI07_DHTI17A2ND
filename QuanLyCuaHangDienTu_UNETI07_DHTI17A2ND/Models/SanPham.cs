namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models
{
    public class SanPham
    {
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public int MaLoai { get; set; }
        public virtual LoaiSanPham? LoaiSanPham { get; set; }
    }
}