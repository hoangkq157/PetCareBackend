using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PetCareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThuCungController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ThuCungController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/thuCung
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ThuCung>>> GetAll()
        {
            return await _context.ThuCungs.ToListAsync();
        }

        // GET: api/thuCung/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ThuCung>> GetById(int id)
        {
            var item = await _context.ThuCungs.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // GET: api/thuCung/bychuNuoi/3  -> lấy thú cưng theo chủ nuôi
        [HttpGet("bychuNuoi/{maCN}")]
        public async Task<ActionResult<IEnumerable<ThuCung>>> GetByChuNuoi(int maCN)
        {
            return await _context.ThuCungs
                .Where(t => t.MaCN == maCN)
                .ToListAsync();
        }

        // POST: api/thuCung
        [HttpPost]
        public async Task<ActionResult<ThuCung>> Create(ThuCung item)
        {
            _context.ThuCungs.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.MaTC }, item);
        }

        // PUT: api/thuCung/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ThuCung item)
        {
            var existing = await _context.ThuCungs.FindAsync(id);
            if (existing == null) return NotFound();

            existing.TenThuCung = item.TenThuCung;
            existing.Loai = item.Loai;
            existing.Giong = item.Giong;
            existing.NgaySinh = item.NgaySinh;
            existing.CanNang = item.CanNang;
            existing.MauLong = item.MauLong;
            existing.GhiChu = item.GhiChu;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/thuCung/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.ThuCungs.FindAsync(id);
            if (item == null) return NotFound();

            _context.ThuCungs.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
