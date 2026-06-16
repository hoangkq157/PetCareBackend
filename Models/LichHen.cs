public class LichHen
{
    public int MaLH { get; set; }
    public int MaTC { get; set; }
    public int? MaNV { get; set; }
    public DateOnly NgayHen { get; set; }
    public TimeOnly GioHen { get; set; }
    public string TrangThai { get; set; } = "ChoDuyet";
    public string? GhiChu { get; set; }
    public DateTime NgayTao { get; set; } = DateTime.Now;
}
