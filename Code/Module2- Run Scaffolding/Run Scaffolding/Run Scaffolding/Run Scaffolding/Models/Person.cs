using System;
using System.Collections.Generic;

namespace Run_Scaffolding.Models;

public partial class Person
{
    public int PersonId { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public byte Gendor { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? ImagePath { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();

    public virtual User? User { get; set; }
}
