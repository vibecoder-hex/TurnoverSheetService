
namespace TurnoverSheetService.Models.DomainEntites;

public partial class Payment
{
    public Guid PaymentUuid { get; private set; }

    public decimal Amount { get; private set; }
    
    public DateOnly PaymentDate { get; private set; }

    public Saldo Saldo { get; set; } = null!;
    public Guid SaldoUuid { get; set; }

    internal Payment(decimal amount, Guid SaldoUuid)
    {
        PaymentDate = DateOnly.FromDateTime(DateTime.Now);
        PaymentUuid = SaldoUuid;
        Amount = amount;
    }
}
