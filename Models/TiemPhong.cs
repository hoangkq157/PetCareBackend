public class TiemPhong
{
    public int MaTP { get; set; }
    public int MaTC { get; set; }
    public string TenVaccine { get; set; } = string.Empty;
    public DateOnly NgayTiem { get; set; }
    public DateOnly? NgayTiemTiep { get; set; }
    public int ChuKyNgay { get; set; } = 365;
    public string? LieuLuong { get; set; }
    public string? BacSiThucHien { get; set; }
    public string? GhiChu { get; set; }
}
