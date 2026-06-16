using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PetCareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichHenDichVuController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LichHenDichVuController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/lichHenDichVu
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LichHenDichVu>>> GetAll()
        {
            return await _context.LichHenDichVus.ToListAsync();
        }

        // GET: api/lichHenDichVu/bylichHen/1  -> dịch vụ của một lịch hẹn
        [HttpGet("bylichHen/{maLH}")]
        public async Task<ActionResult<IEnumerable<LichHenDichVu>>> GetByLichHen(int maLH)
        {
            return await _context.LichHenDichVus
                .Where(l => l.MaLH == maLH)
                .ToListAsync();
        }

        // POST: api/lichHenDichVu
        [HttpPost]
        public async Task<ActionResult<LichHenDichVu>> Create(LichHenDichVu item)
        {
            _context.LichHenDichVus.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = item.MaLHDV }, item);
        }

        // DELETE: api/lichHenDichVu/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.LichHenDichVus.FindAsync(id);
            if (item == null) return NotFound();

            _context.LichHenDichVus.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
