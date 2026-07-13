using System;
using System.Collections.Generic;

namespace Run_Scaffolding.Models;

public partial class User
{
    public int UserId { get; set; }

    public int PersonId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    /// <summary>
    /// 0-Admin 1-Doctor 2-Receptionist
    /// </summary>
    public byte Role { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual Person Person { get; set; } = null!;
}
