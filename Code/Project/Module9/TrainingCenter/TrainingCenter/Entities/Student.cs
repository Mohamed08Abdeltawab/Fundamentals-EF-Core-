using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingCenter.Entities
{
    // Represents Students table
    public class Student
    {
        public int StudentId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }

        public DateTime RegisteredAt { get; set; }

        public string Status { get; set; }
        public string PhoneNumber { get; set; }

        // Navigation Property (Many-to-Many via Enrollments)
        public List<Enrollment> Enrollments { get; set; }
        public StudentProfile? StudentProfile { get; set; }

    }

}
