namespace TurnoverSheetService.Models.DatabaseEntites;

public partial class Apartment
{
    public Guid ApartmentGuid { get; set; }
    public int Number { get; set; }
    
    public string Address { get; set; } = null!;
    
    public ICollection<Saldo> Saldos { get; set; } = new List<Saldo>();
}