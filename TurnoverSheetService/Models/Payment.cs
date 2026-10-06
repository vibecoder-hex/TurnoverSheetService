using System;
using System.Collections.Generic;

namespace TurnoverSheetService;

public partial class Payment
{
    public Guid PaymentUuid { get; set; }

    public int ApartmentNumber { get; set; }

    public DateOnly PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; } = null!;
}
