
namespace TurnoverSheetService.Models;

public partial class Saldo
{
    public Guid SaldoUuid { get; set; }
    
    public string Description { get; set; } = null!;

    public int ApartmentNumber { get; set; }

    public DateOnly PaymentDate { get; set; }

    public decimal IncomingSaldo { get; set; }

    public decimal OutcomingSaldo { get; set; }
}
