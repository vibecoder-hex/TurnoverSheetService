using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Repositories;
using TurnoverSheetService.Models.DatabaseEntites;
using TurnoverSheetService.Models.Domain;

namespace TurnoverSheetService.Services.DomainServices;

public interface ISaldoCalculationService
{
    Task<ServiceResult<string>> CreateNewSaldoOnFutureDate(CreateSaldoDomainDto saldo);
}

public class SaldoCalculationService : ISaldoCalculationService
{
    private readonly ISaldoRepository _saldoRepository;
    private readonly IApartmentsRepository _apartmentsRepository;

    public SaldoCalculationService(ISaldoRepository saldoRepository,  IApartmentsRepository apartmentsRepository)
    {
        _saldoRepository = saldoRepository;
        _apartmentsRepository = apartmentsRepository;
    }

    private decimal CalculateOutcomingSaldo(ReadSaldoDomainDto saldoByLastMonth)
    {
        decimal totalPayment = saldoByLastMonth.TotalPayment;
        decimal totalCharge = saldoByLastMonth.TotalCharge;
        decimal currentIncomingSaldo = saldoByLastMonth.CurrentSaldoValue;
        
        return currentIncomingSaldo + totalCharge - totalPayment;
    }

    private Saldo GetNewSaldoByExistingLastSaldo(ReadSaldoDomainDto? saldoByLastMonth, CreateSaldoDomainDto newSaldoDto, Guid apartmentGuid)
    {
        var newSaldo = new Saldo
        {
            Description = newSaldoDto.Description,
            CalculatingDate = newSaldoDto.CalculatingDate
        };
        
        if (saldoByLastMonth != null)
        {
            newSaldo.CurrentSaldoValue = this.CalculateOutcomingSaldo(saldoByLastMonth);
            newSaldo.ApartmentUuid = saldoByLastMonth.ApartmentGuid;
        }
        else
        {
            newSaldo.CurrentSaldoValue = newSaldoDto.CurrentSaldoValue;
            newSaldo.ApartmentUuid = apartmentGuid;
        }
        return newSaldo;
    }

    public async Task<ServiceResult<string>> CreateNewSaldoOnFutureDate(CreateSaldoDomainDto saldoDto)
    {
        try
        {
            Guid? apartmentGuid = await _apartmentsRepository.ReadApartmentByNumber(saldoDto.ApartmentNumber);
            if (apartmentGuid != Guid.Empty)
            {
                DateOnly lastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));
                var saldoByLastMonth = await _saldoRepository.GetLastSaldoWithTotaledApartmentData(apartmentGuid.Value, lastDate);

                var newSaldo = this.GetNewSaldoByExistingLastSaldo(saldoByLastMonth, saldoDto, apartmentGuid.Value);

                await _saldoRepository.CreateSaldo(newSaldo);
                return ServiceResult<string>.Success("Сальдо по квартире успешно добавлено");
            }
            return ServiceResult<string>.Failure("Квартира по данному номеру не найдена");
        }


        catch (DbUpdateException ex)
        {
            return ServiceResult<string>.Failure(ex.Message);
        }
    }
}