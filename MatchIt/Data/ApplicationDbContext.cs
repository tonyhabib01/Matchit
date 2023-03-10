using MatchIt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace MatchIt.Data
{
    //public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityUserRole, string>
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Tutor> Tutors { get; set; }
        public DbSet<Tutee> Tutees { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<MatchingStudents> MatchingStudents { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<IdentityUser>().ToTable("Users");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");

            modelBuilder.Entity<Student>()
                .HasDiscriminator<string>("StudentType")
                .HasValue<Student>("student")
                .HasValue<Tutor>("tutor")
                .HasValue<Tutee>("tutee");

            modelBuilder.Entity<Course>()
                .HasMany(t => t.MatchingStudents)
                .WithOne(m => m.Course)
                .HasForeignKey("CourseId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
