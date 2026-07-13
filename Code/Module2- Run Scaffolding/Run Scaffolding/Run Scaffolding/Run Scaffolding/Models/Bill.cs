using System;
using System.Collections.Generic;

namespace Run_Scaffolding.Models;

public partial class Bill
{
    public int BillId { get; set; }

    public int VisitId { get; set; }

    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 1-Paid 2-Unpaid
    /// </summary>
    public byte PaymentStatus { get; set; }

    public DateTime PaymentDate { get; set; }

    /// <summary>
    /// 1-Cash 2-CreditCard 3-Insurance
    /// </summary>
    public byte PaymentMethod { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal? Discount { get; set; }

    public int CreatedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual Visit Visit { get; set; } = null!;
}
