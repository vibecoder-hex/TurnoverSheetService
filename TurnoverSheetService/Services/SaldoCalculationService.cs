using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Repositories;
using TurnoverSheetService.Models;

namespace TurnoverSheetService.Services;

public interface ISaldoCalculationService
{
    Task<ServiceResult<string>> CalculateNewSaldoOnFutureDate(CreateApartmentSaldoDto saldo);
}

public class SaldoCalculationService : ISaldoCalculationService
{
    private readonly IApartmentSaldoRepository _saldoRepository;
    private readonly IApartmentPaymentsRepository _paymentsRepository;
    private readonly IApartmentChargesRepository _chargesRepository;

    public SaldoCalculationService(IApartmentSaldoRepository repository, IApartmentPaymentsRepository paymentsRepository, IApartmentChargesRepository chargesRepository)
    {
        _saldoRepository = repository;
        _paymentsRepository = paymentsRepository;
        _chargesRepository = chargesRepository;
    }

    public async Task<ServiceResult<string>> CalculateNewSaldoOnFutureDate(CreateApartmentSaldoDto saldo)
    {
        try
        {
            var lastCalculatedSaldo = await _saldoRepository.ReadLastSaldoByApartmentNumber(saldo.ApartmentNumber);
            if (lastCalculatedSaldo != null)
            {
                int lastSaldoPaymentMonth = lastCalculatedSaldo.PaymentDate.Month;

                var paymentByMonth = await _paymentsRepository.GetPaymentForApartmentByMonth(saldo.ApartmentNumber, lastSaldoPaymentMonth);
                var chargeByMonth = await _chargesRepository.GetChargeForApartmentByMonth(saldo.ApartmentNumber, lastSaldoPaymentMonth);

                if (paymentByMonth != null && chargeByMonth != null)
                {
                    decimal sumOfPayments = paymentByMonth.SumOfAmounts;
                    decimal sumOfCharges = chargeByMonth.SumOfAmounts;
                    
                    decimal lastMonthSaldo = lastCalculatedSaldo.CurrentSaldoValue;
                    decimal futureMonthSaldo = lastMonthSaldo + sumOfCharges - sumOfPayments;
                
                    await _saldoRepository.CreateSaldo(
                        saldo.ApartmentNumber,
                        futureMonthSaldo,
                        saldo.Description);
                }
                else
                {
                    return ServiceResult<string>.Failure(
                        $"Не удалось найти общую сумму начислений/платежей по заданной квартере {saldo.ApartmentNumber}");
                }
            }
            else
            {
                await _saldoRepository.CreateSaldo(saldo.ApartmentNumber, saldo.CurrentSaldoValue, saldo.Description);
            }
            
            return ServiceResult<string>.Success($"Сальдо на квартиру {saldo.ApartmentNumber} успешно создано");
            
        }
        catch (DbUpdateException ex)
        {
            return ServiceResult<string>.Failure(ex.Message);
        }
    }
}