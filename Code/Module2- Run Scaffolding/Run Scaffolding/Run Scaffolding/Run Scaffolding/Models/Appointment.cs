using System;
using System.Collections.Generic;

namespace Run_Scaffolding.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int AppointmentTypeId { get; set; }

    public decimal AppointmentFees { get; set; }

    public DateTime AppointmentDate { get; set; }

    /// <summary>
    /// 1-Scheduled 2-Cancelled 3-Completed
    /// </summary>
    public byte Status { get; set; }

    public int CreatedByUserId { get; set; }

    public virtual AppointmentType AppointmentType { get; set; } = null!;

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual Visit? Visit { get; set; }
}
