using System;
using System.Collections.Generic;

namespace Run_Scaffolding.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public string ServiceDescription { get; set; } = null!;

    public decimal ServicePrice { get; set; }

    public virtual ICollection<VisitService> VisitServices { get; set; } = new List<VisitService>();
}
