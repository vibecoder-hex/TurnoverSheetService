
namespace TurnoverSheetService.Models;

public partial class Charge
{
    public Guid ChargeUuid { get; set; }

    public int ApartmentNumber { get; set; }

    public DateOnly ChargeDate { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; } = null!;
}
