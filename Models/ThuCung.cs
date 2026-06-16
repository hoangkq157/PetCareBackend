public class ThuCung
{
    public int MaTC { get; set; }
    public int MaCN { get; set; }
    public string TenThuCung { get; set; } = string.Empty;
    public string Loai { get; set; } = string.Empty;
    public string? Giong { get; set; }
    public DateOnly? NgaySinh { get; set; }
    public decimal? CanNang { get; set; }
    public string? MauLong { get; set; }
    public string? GhiChu { get; set; }
}
