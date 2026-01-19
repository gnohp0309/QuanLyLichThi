using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThi.API.Models
{
    public class CourseSection
    {
        [Key]
        [StringLength(50)]
        public string SectionCode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string SubjectCode { get; set; } = string.Empty;

        [Required]
        public int TeacherId { get; set; }

        [Required]
        [StringLength(50)]
        public string Semester { get; set; } = string.Empty; // HK1/2025, HK2/2024-2025

        [StringLength(50)]
        public string? DefaultRoom { get; set; }

        public int EnrollmentCount { get; set; } = 0; // Sĩ số

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("SubjectCode")]
        public virtual Subject Subject { get; set; } = null!;

        [ForeignKey("TeacherId")]
        public virtual User Teacher { get; set; } = null!;

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<ExamSchedule> ExamSchedules { get; set; } = new List<ExamSchedule>();
        public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
    }
}
