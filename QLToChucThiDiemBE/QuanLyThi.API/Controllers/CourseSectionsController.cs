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
    public class CourseSectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CourseSectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCourseSections(
            [FromQuery] string? semester, 
            [FromQuery] string? search,
            [FromQuery] int? teacherId)
        {
            var query = _context.CourseSections
                .Include(c => c.Subject)
                .Include(c => c.Teacher)
                .AsQueryable();

            if (!string.IsNullOrEmpty(semester) && semester != "Tất cả")
            {
                query = query.Where(c => c.Semester == semester);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.SectionCode.Contains(search) || 
                                         c.Subject.SubjectName.Contains(search) ||
                                         c.Teacher.FullName.Contains(search));
            }

            if (teacherId.HasValue)
            {
                query = query.Where(c => c.TeacherId == teacherId.Value);
            }

            var sections = await query
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    c.SectionCode,
                    c.SubjectCode,
                    SubjectName = c.Subject.SubjectName,
                    TeacherId = c.TeacherId,
                    TeacherName = c.Teacher.FullName,
                    c.Semester,
                    c.DefaultRoom,
                    c.EnrollmentCount,
                    c.CreatedAt
                })
                .ToListAsync();

            return Ok(sections);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<object>> GetCourseSection(string code)
        {
            var section = await _context.CourseSections
                .Include(c => c.Subject)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(c => c.SectionCode == code);

            if (section == null) return NotFound();

            return Ok(new
            {
                section.SectionCode,
                section.SubjectCode,
                SubjectName = section.Subject.SubjectName,
                TeacherId = section.TeacherId,
                TeacherName = section.Teacher.FullName,
                section.Semester,
                section.DefaultRoom,
                section.EnrollmentCount
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CourseSection>> CreateCourseSection([FromBody] CourseSection section)
        {
            if (await _context.CourseSections.AnyAsync(c => c.SectionCode == section.SectionCode))
            {
                return BadRequest(new { message = "Mã lớp học phần đã tồn tại" });
            }

            if (!await _context.Subjects.AnyAsync(s => s.SubjectCode == section.SubjectCode))
            {
                return BadRequest(new { message = "Mã môn học không tồn tại" });
            }

            if (!await _context.Users.AnyAsync(u => u.Id == section.TeacherId && u.Role == "Teacher"))
            {
                return BadRequest(new { message = "Giảng viên không tồn tại" });
            }

            section.CreatedAt = DateTime.UtcNow;
            section.EnrollmentCount = 0;
            _context.CourseSections.Add(section);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourseSection), new { code = section.SectionCode }, section);
        }

        [HttpPut("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCourseSection(string code, [FromBody] CourseSection section)
        {
            if (code != section.SectionCode)
                return BadRequest();

            var existingSection = await _context.CourseSections.FindAsync(code);
            if (existingSection == null) return NotFound();

            existingSection.SubjectCode = section.SubjectCode;
            existingSection.TeacherId = section.TeacherId;
            existingSection.Semester = section.Semester;
            existingSection.DefaultRoom = section.DefaultRoom;
            existingSection.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourseSection(string code)
        {
            var section = await _context.CourseSections.FindAsync(code);
            if (section == null) return NotFound();

            _context.CourseSections.Remove(section);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
