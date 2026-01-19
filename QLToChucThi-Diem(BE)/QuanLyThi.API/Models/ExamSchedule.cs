using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThi.API.Models
{
    public class ExamSchedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string SectionCode { get; set; } = string.Empty;

        [Required]
        public DateTime ExamDate { get; set; }

        [Required]
        [StringLength(50)]
        public string ExamTime { get; set; } = string.Empty; // 07:00 - 09:00

        [Required]
        [StringLength(50)]
        public string Room { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Location { get; set; } // Địa điểm

        [Required]
        [StringLength(20)]
        public string ExamType { get; set; } = "Exam"; // Exam, Midterm, Final

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("SectionCode")]
        public virtual CourseSection CourseSection { get; set; } = null!;
    }
}
