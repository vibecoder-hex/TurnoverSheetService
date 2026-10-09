using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Models.DatabaseEntites;
using TurnoverSheetService.Models.Domain;
using TurnoverSheetService.Services.DbContextConfiguration;

namespace TurnoverSheetService.Repositories;

public interface IPaymentsRepository
{
    Task CreatePayment(Payment payment);
    Task<List<ReadPaymentDomainDto>> GetAllPaymentsByPeriod(Guid saldoUuid);
}

public class ApartmentPaymentsRepository : IPaymentsRepository
{
    private readonly TurnoverSheetDbContext _dbContext;

    public ApartmentPaymentsRepository(TurnoverSheetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreatePayment(Payment payment)
    {
        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<ReadPaymentDomainDto>> GetAllPaymentsByPeriod(Guid saldoUuid)
    {
        return await _dbContext.Payments
            .AsNoTracking()
            .Where(payment => payment.SaldoUuid == saldoUuid)
            .Select(payment => new ReadPaymentDomainDto(
                payment.SaldoUuid,
                payment.Amount,
                payment.Description,
                payment.PaymentUuid))
            .ToListAsync();
    }
}