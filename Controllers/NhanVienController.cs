using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PetCareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanVienController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NhanVienController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/nhanvien
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanVien>>> GetAll()
        {
            return await _context.NhanViens.ToListAsync();
        }

        // GET: api/nhanvien/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVien>> GetById(int id)
        {
            var item = await _context.NhanViens.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // POST: api/nhanvien
        [HttpPost]
        public async Task<ActionResult<NhanVien>> Create(NhanVien item)
        {
            // Kiểm tra email trùng
            if (await _context.NhanViens.AnyAsync(n => n.Email == item.Email))
                return BadRequest(new { message = "Email này đã được sử dụng." });

            item.NgayTao = DateTime.Now;
            _context.NhanViens.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.MaNV }, item);
        }

        // PUT: api/nhanvien/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, NhanVien item)
        {
            var existing = await _context.NhanViens.FindAsync(id);
            if (existing == null) return NotFound();

            existing.HoTen = item.HoTen;
            existing.SoDienThoai = item.SoDienThoai;
            existing.VaiTro = item.VaiTro;
            existing.TrangThai = item.TrangThai;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/nhanvien/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.NhanViens.FindAsync(id);
            if (item == null) return NotFound();

            _context.NhanViens.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/nhanvien/login
        [HttpPost("login")]
        public async Task<ActionResult<NhanVien>> Login([FromBody] LoginRequest req)
        {
            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(n => n.Email == req.Email && n.MatKhau == req.MatKhau && n.TrangThai);
            if (nhanVien == null)
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });
            return Ok(nhanVien);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
    }
}
