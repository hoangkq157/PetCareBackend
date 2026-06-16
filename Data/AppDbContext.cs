// Data/AppDbContext.cs
// ⚠️  THAY THẾ file gốc - thêm HasIndex unique Email cho ChuNuoi
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<NhanVien>     NhanViens     { get; set; }
    public DbSet<ChuNuoi>      ChuNuois      { get; set; }
    public DbSet<ThuCung>      ThuCungs      { get; set; }
    public DbSet<DichVu>       DichVus       { get; set; }
    public DbSet<LichHen>      LichHens      { get; set; }
    public DbSet<LichHenDichVu> LichHenDichVus { get; set; }
    public DbSet<TiemPhong>    TiemPhongs    { get; set; }
    public DbSet<HoaDon>       HoaDons       { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Tên bảng & khoá chính ──────────────────────────────────
        modelBuilder.Entity<NhanVien>()
            .ToTable("NhanVien")
            .HasKey(e => e.MaNV);

        modelBuilder.Entity<ChuNuoi>()
            .ToTable("ChuNuoi")
            .HasKey(e => e.MaCN);

        // Unique email cho ChuNuoi (khớp với SQL constraint UQ_ChuNuoi_Email)
        modelBuilder.Entity<ChuNuoi>()
            .HasIndex(e => e.Email)
            .IsUnique();

        // Ẩn MatKhau khỏi response GET (tuỳ chọn – bỏ comment nếu muốn)
        // modelBuilder.Entity<ChuNuoi>()
        //     .Ignore(e => e.MatKhau);

        modelBuilder.Entity<ThuCung>()     .ToTable("ThuCung")      .HasKey(e => e.MaTC);
        modelBuilder.Entity<DichVu>()      .ToTable("DichVu")       .HasKey(e => e.MaDV);
        modelBuilder.Entity<LichHen>()     .ToTable("LichHen")      .HasKey(e => e.MaLH);
        modelBuilder.Entity<LichHenDichVu>().ToTable("LichHen_DichVu").HasKey(e => e.MaLHDV);
        modelBuilder.Entity<TiemPhong>()   .ToTable("TiemPhong")    .HasKey(e => e.MaTP);
        modelBuilder.Entity<HoaDon>()      .ToTable("HoaDon")       .HasKey(e => e.MaHD);

        // ── Precision cho decimal ──────────────────────────────────
        modelBuilder.Entity<DichVu>().Property(d => d.GiaCho)  .HasPrecision(10, 0);
        modelBuilder.Entity<DichVu>().Property(d => d.GiaMeo)  .HasPrecision(10, 0);
        modelBuilder.Entity<DichVu>().Property(d => d.GiaKhac) .HasPrecision(10, 0);
        modelBuilder.Entity<ThuCung>().Property(t => t.CanNang).HasPrecision(5, 2);
        modelBuilder.Entity<LichHenDichVu>().Property(l => l.DonGia).HasPrecision(10, 0);
        modelBuilder.Entity<HoaDon>().Property(h => h.TongTien).HasPrecision(12, 0);
    }
}