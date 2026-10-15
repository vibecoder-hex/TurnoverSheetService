using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Models.DomainEntites;
using TurnoverSheetService.Models.Presentation;
using TurnoverSheetService.Services.DbContextConfiguration;

namespace TurnoverSheetService.Repositories;

public interface IRootAggregateCommandRepository
{
    Task CreateApartment(Apartment apartment);
    Task DeleteApartment(int number, string address);
    Task<Apartment?> GetApartmentEntityForExtensions(int apartmentNumber, string address);
    Task SaveChangesAsync();
}

public interface IRootAggregateQueryRepository
{
    Task<bool> IsApartmentExists(int apartmentNumber, string address);
    Task<ReadApartmentResponseDto?> GetApartmentWithTurnoverDetails(int apartmentNumber, string address);
}

public class AggregateCommandRepository : IRootAggregateCommandRepository
{
    private readonly TurnoverSheetDbContext _dbContext;
    
    public AggregateCommandRepository(TurnoverSheetDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task CreateApartment(Apartment apartment) => await _dbContext.Apartments.AddAsync(apartment);
    
    public async Task DeleteApartment(int number, string address)
    {
        await _dbContext.Apartments
            .Where(apartment => apartment.Number == number && apartment.Address == address)
            .ExecuteDeleteAsync();
    }
    
    public async Task<Apartment?> GetApartmentEntityForExtensions(int apartmentNumber, string address)
    {
        return await _dbContext.Apartments
            .Where(apartment => apartment.Number == apartmentNumber && apartment.Address == address)
            .Include(apartment => apartment.Saldos)
                .ThenInclude(saldo => saldo.Charges)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }
    
    public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}

public class AggregateQueryRepository : IRootAggregateQueryRepository
{
    private readonly TurnoverSheetDbContext _dbContext;
    
    public AggregateQueryRepository(TurnoverSheetDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    

    public async Task<bool> IsApartmentExists(int apartmentNumber, string address)
    {
        return await _dbContext.Apartments
            .AsNoTracking()
            .AnyAsync(apartment => apartment.Number == apartmentNumber && apartment.Address == address);
    }

    public async Task<ReadApartmentResponseDto?> GetApartmentWithTurnoverDetails(int number, string address)
    {
        return await _dbContext.Apartments
            .AsNoTracking()
            .Where(apartment => apartment.Number == number && apartment.Address == address)
            .Select(apartment => new ReadApartmentResponseDto(
                apartment.Number,
                apartment.Address,
                apartment.Saldos
                    .Select(saldo => new ReadSaldoResponseDto(
                        saldo.ServiceType,
                        saldo.CurrentSaldoValue,
                        saldo.CalculatingDate,
                        saldo.Charges
                            .Select(charge => new ReadChargeResponseDto(charge.Amount, charge.ChargeDate))
                            .ToList(),
                        saldo.Payments
                            .Select(payment => new ReadPaymentResponseDto(payment.Amount, payment.PaymentDate))
                            .ToList()
                        )
                    )
                    .ToList()
                )
            )
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }
}