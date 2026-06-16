using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace PetCareBackend.Controllers
{
    // ============================================================
    // AuthController – xử lý đăng nhập / đăng ký cho cả hai loại
    // user mà Angular đang gọi:
    //   POST /api/auth/login    ← auth.service.ts gọi
    //   POST /api/auth/register ← auth.service.ts gọi
    // ============================================================
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ── DTOs (chỉ dùng nội bộ controller này) ────────────────────

        public class LoginRequest
        {
            public string Email    { get; set; } = string.Empty;
            public string MatKhau  { get; set; } = string.Empty;
        }

        public class RegisterRequest
        {
            public string  HoTen        { get; set; } = string.Empty;
            public string  Email        { get; set; } = string.Empty;
            public string  MatKhau      { get; set; } = string.Empty;
            public string? SoDienThoai  { get; set; }
            public string? DiaChi       { get; set; }
        }

        // ── Response khớp với UserInfo interface trong Angular ────────
        // UserInfo { token, loai, id, hoTen, vaiTro }
        public class AuthResponse
        {
            public string Token   { get; set; } = string.Empty; // để trống vì chưa dùng JWT thực
            public string Loai    { get; set; } = string.Empty; // "NhanVien" | "ChuNuoi"
            public int    Id      { get; set; }
            public string HoTen   { get; set; } = string.Empty;
            public string VaiTro  { get; set; } = string.Empty; // "Admin" | "NhanVien" | "ChuNuoi"
        }

        // ────────────────────────────────────────────────────────────────
        // POST /api/auth/login
        // Thử tìm trong NhanVien trước, nếu không có thì tìm ChuNuoi
        // ────────────────────────────────────────────────────────────────
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
        {
            // 1. Tìm NhanVien (phải TrangThai = true mới được đăng nhập)
            var nv = await _context.NhanViens
                .FirstOrDefaultAsync(n =>
                    n.Email    == req.Email    &&
                    n.MatKhau  == req.MatKhau  &&
                    n.TrangThai == true);

            if (nv != null)
            {
                return Ok(new AuthResponse
                {
                    Token  = "",           // chưa có JWT – để trống
                    Loai   = "NhanVien",
                    Id     = nv.MaNV,
                    HoTen  = nv.HoTen,
                    VaiTro = nv.VaiTro     // "Admin" hoặc "NhanVien"
                });
            }

            // 2. Tìm ChuNuoi
            var cn = await _context.ChuNuois
                .FirstOrDefaultAsync(c =>
                    c.Email   == req.Email &&
                    c.MatKhau == req.MatKhau);

            if (cn != null)
            {
                return Ok(new AuthResponse
                {
                    Token  = "",
                    Loai   = "ChuNuoi",
                    Id     = cn.MaCN,
                    HoTen  = cn.HoTen,
                    VaiTro = "ChuNuoi"
                });
            }

            // 3. Không tìm thấy
            return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });
        }

        // ────────────────────────────────────────────────────────────────
        // POST /api/auth/register
        // Chỉ đăng ký ChuNuoi (NhanVien do Admin tạo qua /api/nhanvien)
        // ────────────────────────────────────────────────────────────────
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
        {
            // Kiểm tra email đã tồn tại chưa (cả 2 bảng)
            bool emailExists =
                await _context.NhanViens.AnyAsync(n => n.Email == req.Email) ||
                await _context.ChuNuois.AnyAsync(c => c.Email  == req.Email);

            if (emailExists)
                return BadRequest(new { message = "Email này đã được sử dụng." });

            var cn = new ChuNuoi
            {
                HoTen       = req.HoTen,
                Email       = req.Email,
                MatKhau     = req.MatKhau,
                SoDienThoai = req.SoDienThoai ?? string.Empty,
                DiaChi      = req.DiaChi,
                NgayDangKy  = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.ChuNuois.Add(cn);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                Token  = "",
                Loai   = "ChuNuoi",
                Id     = cn.MaCN,
                HoTen  = cn.HoTen,
                VaiTro = "ChuNuoi"
            });
        }
                // GET /api/auth/google-login
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback", "Auth")
            };
            return Challenge(props, "Google");
        }

        // GET /api/auth/google-callback
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync("Cookies");
            if (!result.Succeeded)
                return Redirect("http://localhost:4200/login?error=google_failed");

            var claims = result.Principal!.Claims.ToList();
            var email  = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var hoTen  = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                return Redirect("http://localhost:4200/login?error=no_email");

            // Tìm hoặc tạo ChuNuoi
            var cn = await _context.ChuNuois
                .FirstOrDefaultAsync(c => c.Email == email.ToLower());

            if (cn == null)
            {
                cn = new ChuNuoi
                {
                    HoTen       = hoTen ?? email,
                    Email       = email.ToLower(),
                    MatKhau     = "",
                    SoDienThoai = "",
                    NgayDangKy  = DateOnly.FromDateTime(DateTime.Today)
                };
                _context.ChuNuois.Add(cn);
                await _context.SaveChangesAsync();
            }

            await HttpContext.SignOutAsync("Cookies");

            // Redirect về Angular kèm thông tin user (giống login thường, token để rỗng)
            var redirectUrl = "http://localhost:4200/auth/google/callback" +
                            $"?token=" +
                            $"&hoTen={Uri.EscapeDataString(cn.HoTen)}" +
                            $"&id={cn.MaCN}" +
                            $"&loai=ChuNuoi" +
                            $"&vaiTro=ChuNuoi";

            return Redirect(redirectUrl);
        }
    }
    
}