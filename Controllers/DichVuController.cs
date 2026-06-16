using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PetCareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DichVuController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DichVuController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/dichVu
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DichVu>>> GetAll()
        {
            return await _context.DichVus.Where(d => d.TrangThai).ToListAsync();
        }

        // GET: api/dichVu/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DichVu>> GetById(int id)
        {
            var item = await _context.DichVus.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // GET: api/dichVu/danhmuc/Spa  -> lọc theo danh mục
        [HttpGet("danhmuc/{danhMuc}")]
        public async Task<ActionResult<IEnumerable<DichVu>>> GetByDanhMuc(string danhMuc)
        {
            return await _context.DichVus
                .Where(d => d.DanhMuc == danhMuc && d.TrangThai)
                .ToListAsync();
        }

        // POST: api/dichVu
        [HttpPost]
        public async Task<ActionResult<DichVu>> Create(DichVu item)
        {
            _context.DichVus.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.MaDV }, item);
        }

        // PUT: api/dichVu/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DichVu item)
        {
            var existing = await _context.DichVus.FindAsync(id);
            if (existing == null) return NotFound();

            existing.TenDichVu = item.TenDichVu;
            existing.DanhMuc = item.DanhMuc;
            existing.GiaCho = item.GiaCho;
            existing.GiaMeo = item.GiaMeo;
            existing.GiaKhac = item.GiaKhac;
            existing.MoTa = item.MoTa;
            existing.TrangThai = item.TrangThai;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/dichVu/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.DichVus.FindAsync(id);
            if (item == null) return NotFound();

            _context.DichVus.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
