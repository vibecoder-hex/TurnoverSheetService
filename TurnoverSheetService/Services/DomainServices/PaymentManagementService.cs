using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Models.Domain;
using TurnoverSheetService.Models.DatabaseEntites;
using TurnoverSheetService.Repositories;

namespace TurnoverSheetService.Services.DomainServices;

public interface IPaymentManagementService
{
    Task<ServiceResult<string>> CreatePayment(CreatePaymentDomainDto paymentDto);
}

public class PaymentManagementService : IPaymentManagementService
{
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly ISaldoRepository _saldoRepository;

    public PaymentManagementService(IPaymentsRepository paymentsRepository,  ISaldoRepository saldoRepository)
    {
        _paymentsRepository = paymentsRepository;
        _saldoRepository = saldoRepository;
    }

    public async Task<ServiceResult<string>> CreatePayment(CreatePaymentDomainDto paymentDto)
    {
        try
        {
            Guid saldoUuid = await _saldoRepository.ReadSaldoGuidByDescription(paymentDto.Description);
            if (saldoUuid == Guid.Empty)
                return ServiceResult<string>.Failure("Сальдо по данному типу не существует");
            
            var newPayment = new Payment
            {
                Amount = paymentDto.Amount,
                Description = paymentDto.Description,
                SaldoUuid = saldoUuid
            };
            await _paymentsRepository.CreatePayment(newPayment);
            return ServiceResult<string>.Success("Платеж по расчетному периоду создан");
        }
        catch (DbUpdateException ex)
        {
            return ServiceResult<string>.Failure(ex.Message);
        }
    }
}    