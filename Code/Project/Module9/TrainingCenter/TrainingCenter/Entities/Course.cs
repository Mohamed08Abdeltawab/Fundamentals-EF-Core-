using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingCenter.Entities
{
    // Represents Courses table
    public class Course
    {
        public int CourseId { get; set; }


        public string Title { get; set; }
        public string Code { get; set; }


        public decimal Price { get; set; }
        public string Level { get; set; }


        public int DurationHours { get; set; }


        public string Status { get; set; }


        public int InstructorId { get; set; } // Foreign Key


        // Navigation
        public Instructor Instructor { get; set; }
        public List<Enrollment> Enrollments { get; set; }
    }

}
