using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThi.API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty; // Admin, Teacher, Student

        [StringLength(50)]
        public string? StudentId { get; set; } // MSSV for students

        [StringLength(50)]
        public string? TeacherId { get; set; } // Mã GV for teachers

        [StringLength(100)]
        public string? Major { get; set; } // Ngành học

        [StringLength(100)]
        public string? Faculty { get; set; } // Khoa

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<CourseSection> CourseSections { get; set; } = new List<CourseSection>(); // For teachers
        public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
    }
}
