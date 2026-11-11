using InstituteLMS.API.Models;
using Microsoft.EntityFrameworkCore;
namespace InstituteLMS.API.Data;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
 public DbSet<Branch> Branches=>Set<Branch>(); public DbSet<User> Users=>Set<User>();
 public DbSet<Student> Students=>Set<Student>(); public DbSet<Trainer> Trainers=>Set<Trainer>();
 public DbSet<Course> Courses=>Set<Course>(); public DbSet<Batch> Batches=>Set<Batch>();
 public DbSet<Admission> Admissions=>Set<Admission>(); public DbSet<Attendance> Attendances=>Set<Attendance>();
 public DbSet<FeePayment> FeePayments=>Set<FeePayment>(); public DbSet<Assignment> Assignments=>Set<Assignment>();
 public DbSet<Examination> Examinations=>Set<Examination>(); public DbSet<ExamResult> ExamResults=>Set<ExamResult>();
 protected override void OnModelCreating(ModelBuilder b) {
  base.OnModelCreating(b); b.Entity<User>().HasIndex(x=>x.Email).IsUnique(); b.Entity<Student>().HasIndex(x=>x.StudentCode).IsUnique();
  b.Entity<Course>().Property(x=>x.CourseFee).HasPrecision(18,2); b.Entity<FeePayment>().Property(x=>x.Amount).HasPrecision(18,2);
  b.Entity<Student>().Property(x=>x.Percentage).HasPrecision(5,2); b.Entity<ExamResult>().Property(x=>x.ObtainedMarks).HasPrecision(8,2);
  b.Entity<User>().HasOne(x=>x.Branch).WithMany().HasForeignKey(x=>x.BranchId).OnDelete(DeleteBehavior.Restrict);
  b.Entity<Student>().HasOne(x=>x.User).WithMany().HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Restrict);
  b.Entity<Trainer>().HasOne(x=>x.User).WithMany().HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Restrict);
  b.Entity<Admission>().HasOne(x=>x.Student).WithMany().HasForeignKey(x=>x.StudentId);
  b.Entity<Admission>().HasOne(x=>x.Course).WithMany().HasForeignKey(x=>x.CourseId);
  b.Entity<Admission>().HasOne(x=>x.Branch).WithMany().HasForeignKey(x=>x.BranchId);
 }
}
