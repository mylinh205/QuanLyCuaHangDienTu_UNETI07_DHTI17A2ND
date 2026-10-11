using System.ComponentModel.DataAnnotations;
namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.ViewModels;
public class DatHangVm
{
    [Required] public int MaSanPham { get; set; }
    [Range(1, int.MaxValue, ErrorMessage="Số lượng phải lớn hơn 0")] public int SoLuong { get; set; } = 1;
    [Required(ErrorMessage="Vui lòng nhập địa chỉ giao hàng"), StringLength(255)] public string DiaChiGiaoHang { get; set; } = "";
}
