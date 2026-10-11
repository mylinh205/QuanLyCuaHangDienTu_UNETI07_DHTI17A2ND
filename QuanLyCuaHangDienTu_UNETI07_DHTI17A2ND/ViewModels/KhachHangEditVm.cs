using System.ComponentModel.DataAnnotations;
namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.ViewModels;
public class KhachHangEditVm
{
    [Required, StringLength(100)] public string HoTen { get; set; } = "";
    [DataType(DataType.Date)] public DateTime? NgaySinh { get; set; }
    public string? GioiTinh { get; set; }
    [RegularExpression(@"^0[0-9]{9}$", ErrorMessage="SĐT phải có 10 chữ số và bắt đầu bằng 0")] public string? SoDienThoai { get; set; }
    [EmailAddress] public string? Email { get; set; }
    [StringLength(255)] public string? DiaChi { get; set; }
}
