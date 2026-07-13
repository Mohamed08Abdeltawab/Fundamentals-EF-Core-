using System;
using System.Collections.Generic;

namespace Run_Scaffolding.Models;

public partial class Visit
{
    public int VisitId { get; set; }

    public int AppointmentId { get; set; }

    public DateTime VisitDate { get; set; }

    public string Diagnosis { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Bill? Bill { get; set; }

    public virtual Prescription? Prescription { get; set; }

    public virtual ICollection<VisitService> VisitServices { get; set; } = new List<VisitService>();
}
