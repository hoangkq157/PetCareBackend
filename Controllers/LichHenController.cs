using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PetCareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichHenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LichHenController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/lichHen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LichHen>>> GetAll()
        {
            return await _context.LichHens.ToListAsync();
        }

        // GET: api/lichHen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LichHen>> GetById(int id)
        {
            var item = await _context.LichHens.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // GET: api/lichHen/bythuCung/2  -> lịch hẹn theo thú cưng
        [HttpGet("bythuCung/{maTC}")]
        public async Task<ActionResult<IEnumerable<LichHen>>> GetByThuCung(int maTC)
        {
            return await _context.LichHens
                .Where(l => l.MaTC == maTC)
                .OrderByDescending(l => l.NgayHen)
                .ToListAsync();
        }

        // GET: api/lichHen/bytrangThai/ChoDuyet
        [HttpGet("bytrangThai/{trangThai}")]
        public async Task<ActionResult<IEnumerable<LichHen>>> GetByTrangThai(string trangThai)
        {
            return await _context.LichHens
                .Where(l => l.TrangThai == trangThai)
                .OrderBy(l => l.NgayHen)
                .ToListAsync();
        }

        // POST: api/lichHen
        [HttpPost]
        public async Task<ActionResult<LichHen>> Create(LichHen item)
        {
            item.NgayTao = DateTime.Now;
            item.TrangThai = "ChoDuyet";
            _context.LichHens.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.MaLH }, item);
        }

        // PUT: api/lichHen/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, LichHen item)
        {
            var existing = await _context.LichHens.FindAsync(id);
            if (existing == null) return NotFound();

            existing.MaNV = item.MaNV;
            existing.NgayHen = item.NgayHen;
            existing.GioHen = item.GioHen;
            existing.TrangThai = item.TrangThai;
            existing.GhiChu = item.GhiChu;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/lichHen/5/trangthai  -> chỉ đổi trạng thái
        [HttpPatch("{id}/trangthai")]
        public async Task<IActionResult> UpdateTrangThai(int id, [FromBody] string trangThai)
        {
            var existing = await _context.LichHens.FindAsync(id);
            if (existing == null) return NotFound();

            existing.TrangThai = trangThai;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/lichHen/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.LichHens.FindAsync(id);
            if (item == null) return NotFound();

            _context.LichHens.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
