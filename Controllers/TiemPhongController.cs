using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PetCareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiemPhongController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TiemPhongController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/tiemPhong
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TiemPhong>>> GetAll()
        {
            return await _context.TiemPhongs.ToListAsync();
        }

        // GET: api/tiemPhong/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TiemPhong>> GetById(int id)
        {
            var item = await _context.TiemPhongs.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // GET: api/tiemPhong/bythuCung/1
        [HttpGet("bythuCung/{maTC}")]
        public async Task<ActionResult<IEnumerable<TiemPhong>>> GetByThuCung(int maTC)
        {
            return await _context.TiemPhongs
                .Where(t => t.MaTC == maTC)
                .OrderByDescending(t => t.NgayTiem)
                .ToListAsync();
        }

        // GET: api/tiemPhong/sapdenhan  -> cảnh báo tiêm sắp đến hạn (trong 7 ngày)
        [HttpGet("sapdenhan")]
        public async Task<ActionResult<IEnumerable<TiemPhong>>> GetSapDenHan()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var limit = today.AddDays(7);
            return await _context.TiemPhongs
                .Where(t => t.NgayTiemTiep != null
                         && t.NgayTiemTiep >= today
                         && t.NgayTiemTiep <= limit)
                .OrderBy(t => t.NgayTiemTiep)
                .ToListAsync();
        }

        // POST: api/tiemPhong
        [HttpPost]
        public async Task<ActionResult<TiemPhong>> Create(TiemPhong item)
        {
            // Tự tính ngày tiêm tiếp nếu chưa có
            if (item.NgayTiemTiep == null)
                item.NgayTiemTiep = item.NgayTiem.AddDays(item.ChuKyNgay);

            _context.TiemPhongs.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.MaTP }, item);
        }

        // PUT: api/tiemPhong/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TiemPhong item)
        {
            var existing = await _context.TiemPhongs.FindAsync(id);
            if (existing == null) return NotFound();

            existing.TenVaccine = item.TenVaccine;
            existing.NgayTiem = item.NgayTiem;
            existing.NgayTiemTiep = item.NgayTiemTiep ?? item.NgayTiem.AddDays(item.ChuKyNgay);
            existing.ChuKyNgay = item.ChuKyNgay;
            existing.LieuLuong = item.LieuLuong;
            existing.BacSiThucHien = item.BacSiThucHien;
            existing.GhiChu = item.GhiChu;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/tiemPhong/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.TiemPhongs.FindAsync(id);
            if (item == null) return NotFound();

            _context.TiemPhongs.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
