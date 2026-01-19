using Microsoft.EntityFrameworkCore;
using QuanLyThi.API.Models;

namespace QuanLyThi.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<CourseSection> CourseSections { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<ExamSchedule> ExamSchedules { get; set; }
        public DbSet<Score> Scores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                // PostgreSQL sử dụng WHERE thay vì HasFilter
                entity.HasIndex(e => e.StudentId)
                    .IsUnique()
                    .HasFilter("\"StudentId\" IS NOT NULL");
                entity.HasIndex(e => e.TeacherId)
                    .IsUnique()
                    .HasFilter("\"TeacherId\" IS NOT NULL");
            });

            // Configure CourseSection
            modelBuilder.Entity<CourseSection>(entity =>
            {
                entity.HasIndex(e => e.SectionCode).IsUnique();
                entity.HasOne(c => c.Subject)
                    .WithMany(s => s.CourseSections)
                    .HasForeignKey(c => c.SubjectCode)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.Teacher)
                    .WithMany(u => u.CourseSections)
                    .HasForeignKey(c => c.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Enrollment
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasIndex(e => new { e.StudentId, e.SectionCode }).IsUnique();
                entity.HasOne(e => e.Student)
                    .WithMany(u => u.Enrollments)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.CourseSection)
                    .WithMany(c => c.Enrollments)
                    .HasForeignKey(e => e.SectionCode)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure ExamSchedule
            modelBuilder.Entity<ExamSchedule>(entity =>
            {
                entity.HasOne(e => e.CourseSection)
                    .WithMany(c => c.ExamSchedules)
                    .HasForeignKey(e => e.SectionCode)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Score
            modelBuilder.Entity<Score>(entity =>
            {
                entity.HasIndex(e => new { e.StudentId, e.SectionCode }).IsUnique();
                entity.HasOne(s => s.Student)
                    .WithMany(u => u.Scores)
                    .HasForeignKey(s => s.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(s => s.CourseSection)
                    .WithMany(c => c.Scores)
                    .HasForeignKey(s => s.SectionCode)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
