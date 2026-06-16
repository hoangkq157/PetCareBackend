public class DichVu
{
    public int MaDV { get; set; }
    public string TenDichVu { get; set; } = string.Empty;
    public string? DanhMuc { get; set; }
    public decimal GiaCho { get; set; } = 0;
    public decimal GiaMeo { get; set; } = 0;
    public decimal GiaKhac { get; set; } = 0;
    public string? MoTa { get; set; }
    public bool TrangThai { get; set; } = true;
}
