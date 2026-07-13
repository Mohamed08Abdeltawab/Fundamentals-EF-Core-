using System;
using System.Collections.Generic;

namespace Run_Scaffolding.Models;

public partial class DaysOfWeek
{
    public byte DayId { get; set; }

    public string DayName { get; set; } = null!;

    public virtual ICollection<DoctorWorkingDay> DoctorWorkingDays { get; set; } = new List<DoctorWorkingDay>();
}
