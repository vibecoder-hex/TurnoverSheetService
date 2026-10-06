using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Services.DbContextConfiguration;
using TurnoverSheetService.Models;

namespace TurnoverSheetService.Repositories;

public interface IApartmentSaldoRepository
{
    Task CreateSaldo(int apartmentNumber, decimal incomingSaldo, decimal outcomingSaldo, string description);
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

    public async Task CreateSaldo(int appartmentNumber, decimal incomingSaldo,
        decimal outcomingSaldo, string description)
    {
        var newSaldo = new Saldo
        {
            ApartmentNumber = appartmentNumber,
            PaymentDate = DateOnly.FromDateTime(DateTime.UtcNow),
            IncomingSaldo = incomingSaldo,
            OutcomingSaldo = outcomingSaldo,
            Description = description
        };
        _dbContext.Saldos.Add(newSaldo);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ReadApartmentSaldoDto?> ReadSaldoByDescription(string description)
    {
        var saldo = await _dbContext.Saldos
            .AsNoTracking()
            .Where(saldo => saldo.Description == description)
            .Select(saldo => new ReadApartmentSaldoDto(
                saldo.ApartmentNumber,
                saldo.PaymentDate,
                saldo.IncomingSaldo,
                saldo.OutcomingSaldo,
                saldo.Description
            ))
            .FirstOrDefaultAsync();
        return saldo;
    }

    public async Task<ReadApartmentSaldoDto?> ReadLastSaldoByApartmentNumber(int appartmentNumber)
    {
        var saldo = await _dbContext.Saldos
            .AsNoTracking()
            .Where(saldo => saldo.ApartmentNumber == appartmentNumber)
            .OrderByDescending(saldo => saldo.PaymentDate)
            .Select(saldo => new ReadApartmentSaldoDto(
                saldo.ApartmentNumber,
                saldo.PaymentDate,
                saldo.IncomingSaldo,
                saldo.OutcomingSaldo,
                saldo.Description))
            .FirstOrDefaultAsync();
        return saldo;
    }

public async Task<List<ReadApartmentSaldoDto>> ReadAllSaldos()
    {
        var saldos = await _dbContext.Saldos
            .AsNoTracking()
            .Select(saldo => new ReadApartmentSaldoDto(
                saldo.ApartmentNumber, 
                saldo.PaymentDate,
                saldo.IncomingSaldo,
                saldo.OutcomingSaldo,
                saldo.Description))
            .ToListAsync();
        return saldos;
    }
}