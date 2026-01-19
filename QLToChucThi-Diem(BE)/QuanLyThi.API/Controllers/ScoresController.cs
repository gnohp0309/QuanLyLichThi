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
    public class ScoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ScoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetScores(
            [FromQuery] string? sectionCode,
            [FromQuery] int? studentId)
        {
            var query = _context.Scores
                .Include(s => s.Student)
                .Include(s => s.CourseSection)
                    .ThenInclude(c => c.Subject)
                .AsQueryable();

            if (!string.IsNullOrEmpty(sectionCode))
            {
                query = query.Where(s => s.SectionCode == sectionCode);
            }

            if (studentId.HasValue)
            {
                query = query.Where(s => s.StudentId == studentId.Value);
            }

            var scores = await query
                .Select(s => new
                {
                    s.Id,
                    StudentId = s.StudentId,
                    StudentName = s.Student.FullName,
                    StudentCode = s.Student.StudentId ?? s.Student.Username,
                    s.SectionCode,
                    SubjectName = s.CourseSection.Subject.SubjectName,
                    s.AttendanceScore,
                    s.MidtermScore,
                    s.FinalScore,
                    s.TotalScore,
                    s.Grade,
                    s.Classification,
                    s.IsPublished
                })
                .ToListAsync();

            return Ok(scores);
        }

        [HttpPost("bulk")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> BulkCreateOrUpdateScores([FromBody] BulkScoreRequest request)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            // Verify teacher owns this section
            if (currentUserRole == "Teacher")
            {
                var section = await _context.CourseSections.FindAsync(request.SectionCode);
                if (section == null || section.TeacherId != currentUserId)
                {
                    return Forbid();
                }
            }

            foreach (var scoreData in request.Scores)
            {
                var existingScore = await _context.Scores
                    .FirstOrDefaultAsync(s => s.StudentId == scoreData.StudentId && 
                                             s.SectionCode == request.SectionCode);

                if (existingScore != null)
                {
                    existingScore.AttendanceScore = scoreData.AttendanceScore;
                    existingScore.MidtermScore = scoreData.MidtermScore;
                    existingScore.FinalScore = scoreData.FinalScore;
                    CalculateTotalScore(existingScore);
                    existingScore.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    var newScore = new Score
                    {
                        StudentId = scoreData.StudentId,
                        SectionCode = request.SectionCode,
                        AttendanceScore = scoreData.AttendanceScore,
                        MidtermScore = scoreData.MidtermScore,
                        FinalScore = scoreData.FinalScore,
                        CreatedAt = DateTime.UtcNow
                    };
                    CalculateTotalScore(newScore);
                    _context.Scores.Add(newScore);
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("publish/{sectionCode}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> PublishScores(string sectionCode)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole == "Teacher")
            {
                var section = await _context.CourseSections.FindAsync(sectionCode);
                if (section == null || section.TeacherId != currentUserId)
                {
                    return Forbid();
                }
            }

            var scores = await _context.Scores
                .Where(s => s.SectionCode == sectionCode)
                .ToListAsync();

            foreach (var score in scores)
            {
                score.IsPublished = true;
                score.PublishedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        private void CalculateTotalScore(Score score)
        {
            if (score.AttendanceScore.HasValue && score.MidtermScore.HasValue && score.FinalScore.HasValue)
            {
                score.TotalScore = (score.AttendanceScore.Value * 0.1m) + 
                                   (score.MidtermScore.Value * 0.3m) + 
                                   (score.FinalScore.Value * 0.6m);

                // Determine classification
                if (score.TotalScore >= 9.0m)
                    score.Classification = "Xuất sắc";
                else if (score.TotalScore >= 8.0m)
                    score.Classification = "Giỏi";
                else if (score.TotalScore >= 7.0m)
                    score.Classification = "Khá";
                else if (score.TotalScore >= 5.0m)
                    score.Classification = "Trung bình";
                else
                    score.Classification = "Yếu";

                // Determine grade
                if (score.TotalScore >= 9.0m)
                    score.Grade = "A";
                else if (score.TotalScore >= 8.0m)
                    score.Grade = "B";
                else if (score.TotalScore >= 7.0m)
                    score.Grade = "C";
                else if (score.TotalScore >= 5.0m)
                    score.Grade = "D";
                else
                    score.Grade = "F";
            }
        }
    }

    public class BulkScoreRequest
    {
        public string SectionCode { get; set; } = string.Empty;
        public List<ScoreData> Scores { get; set; } = new();
    }

    public class ScoreData
    {
        public int StudentId { get; set; }
        public decimal? AttendanceScore { get; set; }
        public decimal? MidtermScore { get; set; }
        public decimal? FinalScore { get; set; }
    }
}
