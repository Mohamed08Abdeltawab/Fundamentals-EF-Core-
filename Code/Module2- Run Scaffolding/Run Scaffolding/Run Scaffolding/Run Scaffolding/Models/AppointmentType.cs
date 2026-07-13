using System;
using System.Collections.Generic;

namespace Run_Scaffolding.Models;

public partial class AppointmentType
{
    public int AppointmentTypeId { get; set; }

    public string AppointmentTypeName { get; set; } = null!;

    public decimal AppointmentTypeFees { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
