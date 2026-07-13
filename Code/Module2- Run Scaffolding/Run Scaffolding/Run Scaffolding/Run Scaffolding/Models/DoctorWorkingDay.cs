using System;
using System.Collections.Generic;

namespace Run_Scaffolding.Models;

public partial class DoctorWorkingDay
{
    public int DoctorWorkingDayId { get; set; }

    public int DoctorId { get; set; }

    public byte DayId { get; set; }

    public virtual DaysOfWeek Day { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;
}
