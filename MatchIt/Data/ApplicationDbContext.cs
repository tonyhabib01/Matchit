using MatchIt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace MatchIt.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Tutor> Tutors { get; set; }
        public DbSet<Tutee> Tutees { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<MatchingStudents> MatchingStudents { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasDiscriminator<string>("StudentType")
                .HasValue<Student>("student")
                .HasValue<Tutor>("tutor")
                .HasValue<Tutee>("tutee");

            //modelBuilder.Entity<Tutor>()
            //    .HasMany(t => t.MatchingTutees)
            //    .WithOne(m => m.Tutor)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Tutee>()
            //    .HasMany(t => t.MatchingTutors)
            //    .WithOne(m => m.Tutee)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
