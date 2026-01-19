using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThi.API.Models
{
    public class Score
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [StringLength(50)]
        public string SectionCode { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)")]
        public decimal? AttendanceScore { get; set; } // CC (10%)

        [Column(TypeName = "decimal(5,2)")]
        public decimal? MidtermScore { get; set; } // GK (30%)

        [Column(TypeName = "decimal(5,2)")]
        public decimal? FinalScore { get; set; } // CK (60%)

        [Column(TypeName = "decimal(5,2)")]
        public decimal? TotalScore { get; set; } // Tổng kết

        [StringLength(10)]
        public string? Grade { get; set; } // A, B, C, D, F or Xuất sắc, Giỏi, Khá, Trung bình, Yếu

        [StringLength(10)]
        public string? Classification { get; set; } // Xếp loại: Xuất sắc, Giỏi, Khá, Trung bình, Yếu

        public bool IsPublished { get; set; } = false; // Đã công bố

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? PublishedAt { get; set; }

        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual User Student { get; set; } = null!;

        [ForeignKey("SectionCode")]
        public virtual CourseSection CourseSection { get; set; } = null!;
    }
}
