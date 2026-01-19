using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThi.API.Data;
using QuanLyThi.API.Models;
using System.Security.Claims;

namespace QuanLyThi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetEnrollments([FromQuery] int? studentId, [FromQuery] string? sectionCode)
        {
            var query = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.CourseSection)
                    .ThenInclude(c => c.Subject)
                .Include(e => e.CourseSection)
                    .ThenInclude(c => c.Teacher)
                .AsQueryable();

            if (studentId.HasValue)
            {
                query = query.Where(e => e.StudentId == studentId.Value);
            }

            if (!string.IsNullOrEmpty(sectionCode))
            {
                query = query.Where(e => e.SectionCode == sectionCode);
            }

            var enrollments = await query
                .Select(e => new
                {
                    e.Id,
                    StudentId = e.StudentId,
                    StudentName = e.Student.FullName,
                    StudentCode = e.Student.StudentId ?? e.Student.Username,
                    e.SectionCode,
                    SubjectName = e.CourseSection.Subject.SubjectName,
                    TeacherName = e.CourseSection.Teacher.FullName,
                    e.CourseSection.Semester,
                    e.CourseSection.DefaultRoom,
                    e.Status,
                    e.EnrollmentDate
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        [HttpPost]
        public async Task<ActionResult<Enrollment>> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            // Only students can enroll, or admin can enroll anyone
            int studentIdToUse = currentUserRole == "Admin" ? request.StudentId : currentUserId;

            if (await _context.Enrollments.AnyAsync(e => e.StudentId == studentIdToUse && e.SectionCode == request.SectionCode))
            {
                return BadRequest(new { message = "Đã đăng ký lớp học phần này" });
            }

            var section = await _context.CourseSections.FindAsync(request.SectionCode);
            if (section == null)
            {
                return BadRequest(new { message = "Lớp học phần không tồn tại" });
            }

            var enrollment = new Enrollment
            {
                StudentId = studentIdToUse,
                SectionCode = request.SectionCode,
                Status = "Active",
                EnrollmentDate = DateTime.UtcNow
            };

            section.EnrollmentCount++;
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEnrollments), new { id = enrollment.Id }, enrollment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return NotFound();

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "Admin" && enrollment.StudentId != currentUserId)
            {
                return Forbid();
            }

            var section = await _context.CourseSections.FindAsync(enrollment.SectionCode);
            if (section != null)
            {
                section.EnrollmentCount = Math.Max(0, section.EnrollmentCount - 1);
            }

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class CreateEnrollmentRequest
    {
        public int StudentId { get; set; }
        public string SectionCode { get; set; } = string.Empty;
    }
}
