public class HoaDon
{
    public int MaHD { get; set; }
    public int MaLH { get; set; }
    public int MaCN { get; set; }
    public DateOnly NgayLap { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public decimal TongTien { get; set; } = 0;
    public string TrangThaiTT { get; set; } = "ChuaThanhToan";
    public string? PhuongThucTT { get; set; }
    public string? GhiChu { get; set; }
}
