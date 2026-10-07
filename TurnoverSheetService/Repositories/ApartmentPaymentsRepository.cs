using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Models;
using TurnoverSheetService.Services.DbContextConfiguration;

namespace TurnoverSheetService.Repositories;

public interface IApartmentPaymentsRepository
{
    Task CreatePayment(int apartmentNumber, decimal amount, string description);
    Task<ReadGroupedByFieldsApartmentPaymentsDto?> GetPaymentForApartmentByMonth(int apartmentNumber, int month);
}

public class ApartmentPaymentsRepository : IApartmentPaymentsRepository
{
    private readonly TurnoverSheetDbContext _dbContext;

    public ApartmentPaymentsRepository(TurnoverSheetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreatePayment(int apartmentNumber, decimal amount, string description)
    {
        var payment = new Payment
        {
            ApartmentNumber = apartmentNumber,
            Amount = amount,
            Description = description,
            PaymentDate = DateOnly.FromDateTime(DateTime.Now)
        };
        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ReadGroupedByFieldsApartmentPaymentsDto?> GetPaymentForApartmentByMonth(int apartmentNumber, int month)
    {
        return await _dbContext.Payments
            .AsNoTracking()
            .Where(payment => payment.ApartmentNumber == apartmentNumber && payment.PaymentDate.Month == month)
            .GroupBy(payment => new
            {
                payment.ApartmentNumber,
                payment.PaymentDate.Month
            })
            .Select(group => new ReadGroupedByFieldsApartmentPaymentsDto(
                group.Key.ApartmentNumber,
                group.Key.Month,
                group.Sum(payment => payment.Amount)))
            .FirstOrDefaultAsync();
    }
}