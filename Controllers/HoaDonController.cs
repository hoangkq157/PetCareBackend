using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PetCareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HoaDonController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/hoaDon
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoaDon>>> GetAll()
        {
            return await _context.HoaDons
                .OrderByDescending(h => h.NgayLap)
                .ToListAsync();
        }

        // GET: api/hoaDon/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HoaDon>> GetById(int id)
        {
            var item = await _context.HoaDons.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // GET: api/hoaDon/bychuNuoi/1
        [HttpGet("bychuNuoi/{maCN}")]
        public async Task<ActionResult<IEnumerable<HoaDon>>> GetByChuNuoi(int maCN)
        {
            return await _context.HoaDons
                .Where(h => h.MaCN == maCN)
                .OrderByDescending(h => h.NgayLap)
                .ToListAsync();
        }

        // GET: api/hoaDon/chuathanhtoan
        [HttpGet("chuathanhtoan")]
        public async Task<ActionResult<IEnumerable<HoaDon>>> GetChuaThanhToan()
        {
            return await _context.HoaDons
                .Where(h => h.TrangThaiTT == "ChuaThanhToan")
                .ToListAsync();
        }

        // POST: api/hoaDon
        [HttpPost]
        public async Task<ActionResult<HoaDon>> Create(HoaDon item)
        {
            item.NgayLap = DateOnly.FromDateTime(DateTime.Now);
            item.TrangThaiTT = "ChuaThanhToan";
            _context.HoaDons.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.MaHD }, item);
        }

        // PUT: api/hoaDon/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, HoaDon item)
        {
            var existing = await _context.HoaDons.FindAsync(id);
            if (existing == null) return NotFound();

            existing.TongTien = item.TongTien;
            existing.TrangThaiTT = item.TrangThaiTT;
            existing.PhuongThucTT = item.PhuongThucTT;
            existing.GhiChu = item.GhiChu;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/hoaDon/5/thanhtoan  -> đánh dấu đã thanh toán
        [HttpPatch("{id}/thanhtoan")]
        public async Task<IActionResult> ThanhToan(int id, [FromBody] string phuongThuc)
        {
            var existing = await _context.HoaDons.FindAsync(id);
            if (existing == null) return NotFound();

            existing.TrangThaiTT = "DaThanhToan";
            existing.PhuongThucTT = phuongThuc;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/hoaDon/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.HoaDons.FindAsync(id);
            if (item == null) return NotFound();

            _context.HoaDons.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
