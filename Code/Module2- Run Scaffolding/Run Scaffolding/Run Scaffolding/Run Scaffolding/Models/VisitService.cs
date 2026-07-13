using System;
using System.Collections.Generic;

namespace Run_Scaffolding.Models;

public partial class VisitService
{
    public int VisitServiceId { get; set; }

    public int VisitId { get; set; }

    public int ServiceId { get; set; }

    public decimal ServiceFees { get; set; }

    public virtual Service Service { get; set; } = null!;

    public virtual Visit Visit { get; set; } = null!;
}
