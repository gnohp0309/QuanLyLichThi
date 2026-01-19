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
    public class ExamSchedulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExamSchedulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetExamSchedules(
            [FromQuery] string? sectionCode,
            [FromQuery] string? semester,
            [FromQuery] int? year,
            [FromQuery] int? studentId)
        {
            var query = _context.ExamSchedules
                .Include(e => e.CourseSection)
                    .ThenInclude(c => c.Subject)
                .AsQueryable();

            if (!string.IsNullOrEmpty(sectionCode) && sectionCode != "Tất cả")
            {
                query = query.Where(e => e.SectionCode == sectionCode);
            }

            if (!string.IsNullOrEmpty(semester) && semester != "Tất cả")
            {
                query = query.Where(e => e.CourseSection.Semester == semester);
            }

            if (year.HasValue)
            {
                query = query.Where(e => e.ExamDate.Year == year.Value);
            }

            if (studentId.HasValue)
            {
                var enrolledSections = await _context.Enrollments
                    .Where(e => e.StudentId == studentId.Value && e.Status == "Active")
                    .Select(e => e.SectionCode)
                    .ToListAsync();
                query = query.Where(e => enrolledSections.Contains(e.SectionCode));
            }

            var schedules = await query
                .OrderBy(e => e.ExamDate)
                .ThenBy(e => e.ExamTime)
                .Select(e => new
                {
                    e.Id,
                    e.SectionCode,
                    SubjectName = e.CourseSection.Subject.SubjectName,
                    e.ExamDate,
                    e.ExamTime,
                    e.Room,
                    e.Location,
                    e.ExamType
                })
                .ToListAsync();

            return Ok(schedules);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExamSchedule>> GetExamSchedule(int id)
        {
            var schedule = await _context.ExamSchedules
                .Include(e => e.CourseSection)
                    .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (schedule == null) return NotFound();
            return Ok(schedule);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ExamSchedule>> CreateExamSchedule([FromBody] ExamSchedule schedule)
        {
            if (!await _context.CourseSections.AnyAsync(c => c.SectionCode == schedule.SectionCode))
            {
                return BadRequest(new { message = "Mã lớp học phần không tồn tại" });
            }

            schedule.CreatedAt = DateTime.UtcNow;
            _context.ExamSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExamSchedule), new { id = schedule.Id }, schedule);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateExamSchedule(int id, [FromBody] ExamSchedule schedule)
        {
            if (id != schedule.Id)
                return BadRequest();

            var existingSchedule = await _context.ExamSchedules.FindAsync(id);
            if (existingSchedule == null) return NotFound();

            existingSchedule.SectionCode = schedule.SectionCode;
            existingSchedule.ExamDate = schedule.ExamDate;
            existingSchedule.ExamTime = schedule.ExamTime;
            existingSchedule.Room = schedule.Room;
            existingSchedule.Location = schedule.Location;
            existingSchedule.ExamType = schedule.ExamType;
            existingSchedule.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteExamSchedule(int id)
        {
            var schedule = await _context.ExamSchedules.FindAsync(id);
            if (schedule == null) return NotFound();

            _context.ExamSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
