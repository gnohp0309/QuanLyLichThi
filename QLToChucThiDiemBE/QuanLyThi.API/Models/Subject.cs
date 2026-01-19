using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThi.API.Models
{
    public class Subject
    {
        [Key]
        [StringLength(20)]
        public string SubjectCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string SubjectName { get; set; } = string.Empty;

        [Required]
        public int Credits { get; set; } // Số tín chỉ

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<CourseSection> CourseSections { get; set; } = new List<CourseSection>();
    }
}
