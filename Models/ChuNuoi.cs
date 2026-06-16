public class ChuNuoi
{
    public int MaCN { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
     public string MatKhau { get; set; } = string.Empty;  
    public string? DiaChi { get; set; }
    public DateOnly NgayDangKy { get; set; } = DateOnly.FromDateTime(DateTime.Now);
}
