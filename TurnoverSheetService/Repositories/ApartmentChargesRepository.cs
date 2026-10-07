using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Models;
using TurnoverSheetService.Services.DbContextConfiguration;

namespace TurnoverSheetService.Repositories;

public interface IApartmentChargesRepository
{
    Task CreateCharge(int apartmentNumber, decimal amount, string description);
    Task<ReadApartmentChargeDto?> GetChargeByDescription(string description);
    Task<ReadGroupedByFieldsApartmentChargeDto?> GetChargeForApartmentByMonth(int appartmentNumber, int month);
}

public class ApartmentChargesRepository : IApartmentChargesRepository
{
    private readonly TurnoverSheetDbContext _dbContext;
    
    public ApartmentChargesRepository(TurnoverSheetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateCharge(int apartmentNumber, decimal amount, string description)
    {
        var charge = new Charge
        {
            ApartmentNumber = apartmentNumber,
            Amount = amount,
            Description = description,
            ChargeDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        _dbContext.Charges.Add(charge);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ReadApartmentChargeDto?> GetChargeByDescription(string description)
    {
        return await _dbContext.Charges
            .AsNoTracking()
            .Where(charge => charge.Description == description)
            .Select(charge => new ReadApartmentChargeDto(
                charge.ApartmentNumber,
                charge.ChargeDate,
                charge.Amount,
                charge.Description))
            .FirstOrDefaultAsync();
    }

    public async Task<ReadGroupedByFieldsApartmentChargeDto?> GetChargeForApartmentByMonth(int appartmentNumber, int month)
    {
        return await _dbContext.Charges
            .AsNoTracking()
            .Where(charge => charge.ApartmentNumber == appartmentNumber && charge.ChargeDate.Month == month)
            .GroupBy(charge => new
            {
                charge.ApartmentNumber,
                charge.ChargeDate.Month
            })
            .Select(group => new ReadGroupedByFieldsApartmentChargeDto(
                group.Key.ApartmentNumber,
                group.Key.Month,
                group.Sum(charge => charge.Amount)))
            .FirstOrDefaultAsync();
    }
}