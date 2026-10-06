using System;
using System.Collections.Generic;

namespace TurnoverSheetService;

public partial class Saldo
{
    public Guid SaldoUuid { get; set; }

    public int ApartmentNumber { get; set; }

    public DateOnly PaymentDate { get; set; }

    public decimal IncomingSaldo { get; set; }

    public decimal OutcomingSaldo { get; set; }
}
