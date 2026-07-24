using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Import EF Core namespace
using Microsoft.EntityFrameworkCore;


// Import entity namespace
using TrainingCenter.Entities;


namespace TrainingCenter.Data
{
    // AppDbContext represents the connection/session with the database
    public class AppDbContext : DbContext
    {
        // Constructor receives configuration options and passes them to the base DbContext class
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }


        // Each DbSet represents a table in the database
        public DbSet<Student> Students { get; set; }


        public DbSet<Course> Courses { get; set; }


        public DbSet<Instructor> Instructors { get; set; }


        public DbSet<Enrollment> Enrollments { get; set; }


        public DbSet<StudentProfile> StudentProfiles { get; set; }




//👉 EF Core is using default conventions only
//🧠 Why This Breaks

//EF Core tries to guess the primary key using naming conventions:

//👉 It expects:

//Id
//StudentProfileId

//❌ But your key is:

//StudentId

//👉 EF cannot detect it automatically → ERROR


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure StudentProfile primary key
            modelBuilder.Entity<StudentProfile>(entity =>
            {
                entity.HasKey(e => e.StudentId); 
            });
        }


    }


}
