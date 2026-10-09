
namespace TurnoverSheetService.Models.DatabaseEntites;

public partial class Charge
{
    public Guid ChargeUuid { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; } = null!;

    public Saldo Saldo { get; set; } = null!;
    public Guid SaldoUuid { get; set; }
}
