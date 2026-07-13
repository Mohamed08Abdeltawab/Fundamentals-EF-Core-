using System;
using System.Collections.Generic;

namespace Run_Scaffolding.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public int PersonId { get; set; }

    public string Specialization { get; set; } = null!;

    public decimal ConsultationFees { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<DoctorWorkingDay> DoctorWorkingDays { get; set; } = new List<DoctorWorkingDay>();

    public virtual Person Person { get; set; } = null!;
}
