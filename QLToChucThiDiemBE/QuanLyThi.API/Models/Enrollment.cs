using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThi.API.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [StringLength(50)]
        public string SectionCode { get; set; } = string.Empty;

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Active"; // Active, Completed, Dropped

        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual User Student { get; set; } = null!;

        [ForeignKey("SectionCode")]
        public virtual CourseSection CourseSection { get; set; } = null!;
    }
}
