using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Models.DatabaseEntites;
using TurnoverSheetService.Services.DbContextConfiguration;

namespace TurnoverSheetService.Repositories;

public interface IChargesRepository
{
    Task CreateCharge(Charge newCharge);
}

public class ApartmentChargesRepository : IChargesRepository
{
    private readonly TurnoverSheetDbContext _dbContext;
    
    public ApartmentChargesRepository(TurnoverSheetDbContext dbContext) => _dbContext = dbContext;

    public async Task CreateCharge(Charge newCharge) => await _dbContext.Charges.AddAsync(newCharge);
    
}