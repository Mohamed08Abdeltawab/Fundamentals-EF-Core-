using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingCenter.Entities
{
    // Represents Instructors table
    public class Instructor
    {
        public int InstructorId { get; set; }


        public string FirstName { get; set; }
        public string LastName { get; set; }


        public decimal Salary { get; set; }


        public int? ManagerId { get; set; } // Self-reference


        // Navigation
        public Instructor Manager { get; set; }
        public List<Course> Courses { get; set; }
    }

}
