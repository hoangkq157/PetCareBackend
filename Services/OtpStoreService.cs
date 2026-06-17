using Microsoft.Extensions.Caching.Memory;

namespace PetCareBackend.Services;

public class OtpEntry
{
    public string Code     { get; set; } = string.Empty;
    public string Email    { get; set; } = string.Empty;
    public string LoaiTk   { get; set; } = string.Empty; // "ChuNuoi" | "NhanVien"
    public bool   Verified { get; set; } = false;
}

public interface IOtpStoreService
{
    string TaoOtp(string email, string loaiTk);
    OtpEntry? Lay(string email);
    bool XacMinh(string email, string otp);
    bool DaXacMinh(string email);
    void Xoa(string email);
}

// Lưu OTP tạm thời trong bộ nhớ server, key = email, tự hết hạn sau 5 phút.
// Phù hợp cho REST API stateless (Angular không dùng cookie session).
public class OtpStoreService : IOtpStoreService
{
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan ThoiHan = TimeSpan.FromMinutes(5);

    public OtpStoreService(IMemoryCache cache)
    {
        _cache = cache;
    }

    private static string Key(string email) => $"otp:{email.Trim().ToLower()}";

    public string TaoOtp(string email, string loaiTk)
    {
        var otp = new Random().Next(100000, 999999).ToString();
        var entry = new OtpEntry
        {
            Code     = otp,
            Email    = email.Trim().ToLower(),
            LoaiTk   = loaiTk,
            Verified = false
        };

        _cache.Set(Key(email), entry, ThoiHan);
        return otp;
    }

    public OtpEntry? Lay(string email) => _cache.Get<OtpEntry>(Key(email));

    public bool XacMinh(string email, string otp)
    {
        var entry = Lay(email);
        if (entry == null) return false;
        if (entry.Code != otp?.Trim()) return false;

        entry.Verified = true;
        _cache.Set(Key(email), entry, ThoiHan); // ghi lại để giữ trạng thái verified
        return true;
    }

    public bool DaXacMinh(string email)
    {
        var entry = Lay(email);
        return entry != null && entry.Verified;
    }

    public void Xoa(string email) => _cache.Remove(Key(email));
}