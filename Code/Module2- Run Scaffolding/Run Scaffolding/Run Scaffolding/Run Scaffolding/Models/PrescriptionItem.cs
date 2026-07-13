using System;
using System.Collections.Generic;

namespace Run_Scaffolding.Models;

public partial class PrescriptionItem
{
    public int ItemId { get; set; }

    public int PrescriptionId { get; set; }

    public int MedicineId { get; set; }

    public int? Quantity { get; set; }

    public string? Dosage { get; set; }

    public string? Instructions { get; set; }

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;
}
