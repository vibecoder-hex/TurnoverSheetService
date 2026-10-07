using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Services.DbContextConfiguration;
using TurnoverSheetService.Models;

namespace TurnoverSheetService.Repositories;

public interface IApartmentSaldoRepository
{
    Task CreateSaldo(int apartmentNumber, decimal currentSaldoValue, string description);
    Task<ReadApartmentSaldoDto?> ReadSaldoByDescription(string description);
    Task<ReadApartmentSaldoDto?> ReadLastSaldoByApartmentNumber(int appartmentNumber);
    Task<List<ReadApartmentSaldoDto>> ReadAllSaldos();
}

public class ApartmentSaldoRepository : IApartmentSaldoRepository
{
    private readonly TurnoverSheetDbContext _dbContext;

    public ApartmentSaldoRepository(TurnoverSheetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateSaldo(int appartmentNumber, decimal currentSaldoValue, string description)
    {
        var newSaldo = new Saldo
        {
            ApartmentNumber = appartmentNumber,
            PaymentDate = DateOnly.FromDateTime(DateTime.UtcNow),
            CurrentSaldoValue = currentSaldoValue,
            Description = description
        };
        _dbContext.Saldos.Add(newSaldo);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ReadApartmentSaldoDto?> ReadSaldoByDescription(string description)
    {
        return await _dbContext.Saldos
            .AsNoTracking()
            .Where(saldo => saldo.Description == description)
            .Select(saldo => new ReadApartmentSaldoDto(
                saldo.ApartmentNumber,
                saldo.PaymentDate,
                saldo.CurrentSaldoValue,
                saldo.Description
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<ReadApartmentSaldoDto?> ReadLastSaldoByApartmentNumber(int appartmentNumber)
    {
        return await _dbContext.Saldos
            .AsNoTracking()
            .Where(saldo => saldo.ApartmentNumber == appartmentNumber)
            .OrderByDescending(saldo => saldo.PaymentDate)
            .Select(saldo => new ReadApartmentSaldoDto(
                saldo.ApartmentNumber,
                saldo.PaymentDate,
                saldo.CurrentSaldoValue,
                saldo.Description))
            .FirstOrDefaultAsync();
    }

    public async Task<List<ReadApartmentSaldoDto>> ReadAllSaldos()
    {
        return await _dbContext.Saldos
            .AsNoTracking()
            .Select(saldo => new ReadApartmentSaldoDto(
                saldo.ApartmentNumber,
                saldo.PaymentDate, 
                saldo.CurrentSaldoValue,
                saldo.Description))
            .ToListAsync();
    }
}