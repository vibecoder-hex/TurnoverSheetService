
namespace TurnoverSheetService.Models.DomainEntites;

public partial class Charge
{
    public Guid ChargeUuid { get; private set; }

    public decimal Amount { get; private set; }
    
    public DateOnly ChargeDate { get; private set; }
    
    public Saldo Saldo { get; private set; } = null!;
    public Guid SaldoUuid { get; private set; }
    
    internal Charge(decimal amount, Guid saldoUuid)
    {
        Amount = amount;
       ChargeDate = DateOnly.FromDateTime(DateTime.UtcNow);
       SaldoUuid = saldoUuid;
    }
}
