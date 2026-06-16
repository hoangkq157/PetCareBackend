public class NhanVien
{
    public int MaNV { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
    public string VaiTro { get; set; } = "NhanVien";
    public string? SoDienThoai { get; set; }
    public DateTime NgayTao { get; set; } = DateTime.Now;
    public bool TrangThai { get; set; } = true;
}
