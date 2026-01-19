using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThi.API.Data;
using QuanLyThi.API.Models;

namespace QuanLyThi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subject>>> GetSubjects([FromQuery] string? search)
        {
            var query = _context.Subjects.AsQueryable();
            
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.SubjectCode.Contains(search) || 
                                         s.SubjectName.Contains(search));
            }

            return Ok(await query.OrderBy(s => s.SubjectCode).ToListAsync());
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<Subject>> GetSubject(string code)
        {
            var subject = await _context.Subjects.FindAsync(code);
            if (subject == null) return NotFound();
            return Ok(subject);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Subject>> CreateSubject([FromBody] Subject subject)
        {
            if (await _context.Subjects.AnyAsync(s => s.SubjectCode == subject.SubjectCode))
            {
                return BadRequest(new { message = "Mã môn học đã tồn tại" });
            }

            subject.CreatedAt = DateTime.UtcNow;
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubject), new { code = subject.SubjectCode }, subject);
        }

        [HttpPut("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubject(string code, [FromBody] Subject subject)
        {
            if (code != subject.SubjectCode)
                return BadRequest();

            var existingSubject = await _context.Subjects.FindAsync(code);
            if (existingSubject == null) return NotFound();

            existingSubject.SubjectName = subject.SubjectName;
            existingSubject.Credits = subject.Credits;
            existingSubject.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubject(string code)
        {
            var subject = await _context.Subjects.FindAsync(code);
            if (subject == null) return NotFound();

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
