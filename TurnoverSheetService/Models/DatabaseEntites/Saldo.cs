
namespace TurnoverSheetService.Models.DatabaseEntites;

public partial class Saldo
{
    public Guid SaldoUuid { get; set; }
    
    public string Description { get; set; } = null!;

    public Apartment Apartment { get; set; } = null!;
    
    public Guid ApartmentUuid { get; set; }

    public DateOnly CalculatingDate { get; set; }

    public decimal CurrentSaldoValue { get; set; }
    
    public ICollection<Charge> Charges { get; set; } = new List<Charge>();
    
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
