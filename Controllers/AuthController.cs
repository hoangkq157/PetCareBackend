using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareBackend.Services;

namespace PetCareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext     _context;
        private readonly IEmailService    _emailService;
        private readonly IOtpStoreService _otpStore;

        public AuthController(
            AppDbContext context,
            IEmailService emailService,
            IOtpStoreService otpStore)
        {
            _context      = context;
            _emailService = emailService;
            _otpStore     = otpStore;
        }

        // ── DTOs ──────────────────────────────────────────────────────

        public class LoginRequest
        {
            public string Email   { get; set; } = string.Empty;
            public string MatKhau { get; set; } = string.Empty;
        }

        public class RegisterRequest
        {
            public string  HoTen       { get; set; } = string.Empty;
            public string  Email       { get; set; } = string.Empty;
            public string  MatKhau     { get; set; } = string.Empty;
            public string? SoDienThoai { get; set; }
            public string? DiaChi      { get; set; }
        }

        public class QuenMatKhauRequest
        {
            public string Email { get; set; } = string.Empty;
        }

        public class XacMinhOtpRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Otp   { get; set; } = string.Empty;
        }

        public class DatLaiMatKhauRequest
        {
            public string Email          { get; set; } = string.Empty;
            public string MatKhauMoi     { get; set; } = string.Empty;
            public string XacNhanMatKhau { get; set; } = string.Empty;
        }

        public class AuthResponse
        {
            public string Token   { get; set; } = string.Empty;
            public string Loai    { get; set; } = string.Empty; // "NhanVien" | "ChuNuoi"
            public int    Id      { get; set; }
            public string HoTen   { get; set; } = string.Empty;
            public string VaiTro  { get; set; } = string.Empty;
        }

        // ────────────────────────────────────────────────────────────────
        // POST /api/auth/login
        // ────────────────────────────────────────────────────────────────
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
        {
            var nv = await _context.NhanViens
                .FirstOrDefaultAsync(n =>
                    n.Email     == req.Email   &&
                    n.MatKhau   == req.MatKhau &&
                    n.TrangThai == true);

            if (nv != null)
            {
                return Ok(new AuthResponse
                {
                    Token  = "",
                    Loai   = "NhanVien",
                    Id     = nv.MaNV,
                    HoTen  = nv.HoTen,
                    VaiTro = nv.VaiTro
                });
            }

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

            return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });
        }

        // ────────────────────────────────────────────────────────────────
        // POST /api/auth/register
        // ────────────────────────────────────────────────────────────────
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
        {
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

        // ────────────────────────────────────────────────────────────────
        // POST /api/auth/quenmatkhau  – Bước 1: gửi OTP về email
        // ────────────────────────────────────────────────────────────────
        [HttpPost("quenmatkhau")]
        public async Task<IActionResult> QuenMatKhau([FromBody] QuenMatKhauRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email))
                return BadRequest(new { message = "Vui lòng nhập địa chỉ email." });

            var email = req.Email.Trim().ToLower();

            string? hoTen  = null;
            string? loaiTk = null;

            var cn = await _context.ChuNuois.FirstOrDefaultAsync(c => c.Email == email);
            if (cn != null)
            {
                hoTen  = cn.HoTen;
                loaiTk = "ChuNuoi";
            }
            else
            {
                var nv = await _context.NhanViens
                    .FirstOrDefaultAsync(n => n.Email == email && n.TrangThai == true);
                if (nv != null)
                {
                    hoTen  = nv.HoTen;
                    loaiTk = "NhanVien";
                }
            }

            // Luôn trả về message giống nhau để tránh lộ thông tin tài khoản
            var thongBao = "Nếu email tồn tại trong hệ thống, mã OTP đã được gửi. Vui lòng kiểm tra hộp thư (kể cả thư mục Spam).";

            if (hoTen != null && loaiTk != null)
            {
                var otp = _otpStore.TaoOtp(email, loaiTk);

                try
                {
                    await _emailService.SendOtpEmailAsync(email, hoTen, otp);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = $"Không thể gửi email: {ex.Message}" });
                }
            }

            return Ok(new { message = thongBao });
        }

        // ────────────────────────────────────────────────────────────────
        // POST /api/auth/xacminhotp  – Bước 2: xác minh mã OTP
        // ────────────────────────────────────────────────────────────────
        [HttpPost("xacminhotp")]
        public IActionResult XacMinhOtp([FromBody] XacMinhOtpRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Otp))
                return BadRequest(new { message = "Thiếu email hoặc mã OTP." });

            var email = req.Email.Trim().ToLower();
            var entry = _otpStore.Lay(email);

            if (entry == null)
                return BadRequest(new { message = "Mã OTP đã hết hạn hoặc không tồn tại. Vui lòng yêu cầu mã mới." });

            bool hopLe = _otpStore.XacMinh(email, req.Otp);
            if (!hopLe)
                return BadRequest(new { message = "Mã OTP không đúng. Vui lòng kiểm tra lại." });

            return Ok(new { message = "Xác minh OTP thành công." });
        }

        // ────────────────────────────────────────────────────────────────
        // POST /api/auth/datlaimatkhau  – Bước 3: đặt mật khẩu mới
        // ────────────────────────────────────────────────────────────────
        [HttpPost("datlaimatkhau")]
        public async Task<IActionResult> DatLaiMatKhau([FromBody] DatLaiMatKhauRequest req)
        {
            var email = req.Email.Trim().ToLower();

            if (!_otpStore.DaXacMinh(email))
                return BadRequest(new { message = "Bạn cần xác minh OTP trước khi đặt lại mật khẩu." });

            if (string.IsNullOrWhiteSpace(req.MatKhauMoi) || req.MatKhauMoi.Length < 6)
                return BadRequest(new { message = "Mật khẩu phải có ít nhất 6 ký tự." });

            if (req.MatKhauMoi != req.XacNhanMatKhau)
                return BadRequest(new { message = "Mật khẩu xác nhận không khớp." });

            var entry = _otpStore.Lay(email);
            if (entry == null)
                return BadRequest(new { message = "Phiên xác minh đã hết hạn. Vui lòng thử lại từ đầu." });

            bool daCapNhat = false;

            if (entry.LoaiTk == "ChuNuoi")
            {
                var cn = await _context.ChuNuois.FirstOrDefaultAsync(c => c.Email == email);
                if (cn != null)
                {
                    cn.MatKhau = req.MatKhauMoi;
                    await _context.SaveChangesAsync();
                    daCapNhat = true;
                }
            }
            else if (entry.LoaiTk == "NhanVien")
            {
                var nv = await _context.NhanViens
                    .FirstOrDefaultAsync(n => n.Email == email && n.TrangThai == true);
                if (nv != null)
                {
                    nv.MatKhau = req.MatKhauMoi;
                    await _context.SaveChangesAsync();
                    daCapNhat = true;
                }
            }

            if (!daCapNhat)
                return BadRequest(new { message = "Không tìm thấy tài khoản." });

            _otpStore.Xoa(email);

            return Ok(new { message = "Mật khẩu đã được đặt lại thành công. Vui lòng đăng nhập." });
        }
    }
}