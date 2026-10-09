using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Services.DbContextConfiguration;
using TurnoverSheetService.Models.DatabaseEntites;
using TurnoverSheetService.Models.Domain;

namespace TurnoverSheetService.Repositories;

public interface ISaldoRepository
{
    Task CreateSaldo(Saldo newSaldo);
    Task<ReadSaldoDomainDto?> GetLastSaldoWithTotaledApartmentData(Guid apartmentNumber, DateOnly lastDate);
    Task<Guid> ReadSaldoGuidByDescription(string description);
}

public class ApartmentSaldoRepository : ISaldoRepository
{
    private readonly TurnoverSheetDbContext _dbContext;

    public ApartmentSaldoRepository(TurnoverSheetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateSaldo(Saldo newSaldo)
    {
        _dbContext.Saldos.Add(newSaldo);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ReadSaldoDomainDto?> GetLastSaldoWithTotaledApartmentData(Guid apartmentUuid, DateOnly lastDate)
    {
        return await _dbContext.Saldos
            .AsNoTracking()
            .Where(saldo => saldo.ApartmentUuid == apartmentUuid && 
                            saldo.CalculatingDate.Month == lastDate.Month &&
                            saldo.CalculatingDate.Year == lastDate.Year)
            .Select(saldo => new ReadSaldoDomainDto(
                saldo.SaldoUuid,
                saldo.CalculatingDate,
                saldo.CurrentSaldoValue,
                saldo.Description,
                saldo.Charges.Sum(charge => charge.Amount),
                saldo.Payments.Sum(payment => payment.Amount),
                saldo.ApartmentUuid))
            .FirstOrDefaultAsync();
    }

    public async Task<Guid> ReadSaldoGuidByDescription(string description)
    {
        return await _dbContext.Saldos
            .AsNoTracking()
            .Where(saldo => saldo.Description == description)
            .Select(saldo => saldo.SaldoUuid)
            .FirstOrDefaultAsync();
    }
}