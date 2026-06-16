using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PetCareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChuNuoiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChuNuoiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/chuNuoi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChuNuoi>>> GetAll()
        {
            return await _context.ChuNuois.ToListAsync();
        }

        // GET: api/chuNuoi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChuNuoi>> GetById(int id)
        {
            var item = await _context.ChuNuois.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // POST: api/chuNuoi
        [HttpPost]
        public async Task<ActionResult<ChuNuoi>> Create(ChuNuoi item)
        {
            item.NgayDangKy = DateOnly.FromDateTime(DateTime.Now);
            _context.ChuNuois.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.MaCN }, item);
        }

        // PUT: api/chuNuoi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ChuNuoi item)
        {
            var existing = await _context.ChuNuois.FindAsync(id);
            if (existing == null) return NotFound();

            existing.HoTen = item.HoTen;
            existing.SoDienThoai = item.SoDienThoai;
            existing.Email = item.Email;
            existing.DiaChi = item.DiaChi;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/chuNuoi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.ChuNuois.FindAsync(id);
            if (item == null) return NotFound();

            _context.ChuNuois.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
