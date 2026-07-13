using System;
using System.Collections.Generic;

namespace Run_Scaffolding.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public int PersonId { get; set; }

    public string? MedicalHistory { get; set; }

    /// <summary>
    /// 0-Unknown 1-A+ 2-A- 3-B+ 4-B+ 5-AB+ 6-AB- 7-O+ 8-O- 
    /// </summary>
    public byte? BloodType { get; set; }

    public string EmergencyContact { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Person Person { get; set; } = null!;
}
