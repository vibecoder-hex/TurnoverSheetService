using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Models.DatabaseEntites;
using TurnoverSheetService.Models.Domain;
using TurnoverSheetService.Services.DbContextConfiguration;

namespace TurnoverSheetService.Repositories;

public interface IApartmentsRepository
{
    Task CreateApartment(Apartment apartment);
    
    Task<Guid> ReadApartmentByNumber(int apartmentNumber);
}

public class ApartmentsRepository : IApartmentsRepository
{
    private readonly TurnoverSheetDbContext _dbContext;
    
    public ApartmentsRepository(TurnoverSheetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateApartment(Apartment apartment)
    {
        _dbContext.Apartments.Add(apartment);
        await _dbContext.SaveChangesAsync();
    } 

    public async Task<Guid> ReadApartmentByNumber(int apartmentNumber)
    {
        return await _dbContext.Apartments
            .AsNoTracking()
            .Where(apartment => apartment.Number == apartmentNumber)
            .Select(apartment => apartment.ApartmentGuid)
            .FirstOrDefaultAsync();
    }
    
}