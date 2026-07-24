using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingCenter.Entities
{
    // Represents StudentProfiles table
    public class StudentProfile
    {
        public int StudentId { get; set; } // PK & FK


        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }


        // Navigation
        public Student Student { get; set; }
    }
}
