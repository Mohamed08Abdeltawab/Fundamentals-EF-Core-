using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingCenter.Entities
{
    // Represents Enrollments table (Many-to-Many)
    public class Enrollment
    {
        public int EnrollmentId { get; set; }


        public int StudentId { get; set; }
        public int CourseId { get; set; }


        public decimal ProgressPercent { get; set; }
        public decimal? FinalGrade { get; set; }


        public string Status { get; set; }


        // Navigation
        public Student Student { get; set; }
        public Course Course { get; set; }
    }

}
