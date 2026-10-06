using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Repositories;
using TurnoverSheetService.Models;

namespace TurnoverSheetService.Services;

public class ServiceResult<T>
{
    private T? _resultObject;
    private string? _errorMessage;
    private bool _isSuccess;
    

    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>
        {
            _isSuccess =  true,
            _resultObject = data
        };
    }

    public static ServiceResult<T> Failure(string errorMessage)
    {
        return new ServiceResult<T>
        {
            _isSuccess = false,
            _errorMessage = errorMessage
        };
    }
}

public interface ISaldoCalculationService
{
    Task<ServiceResult<string>> CalculateNewOutcomingSaldoByIncoming(CreateApartmentSaldoDto saldo);
}

public class SaldoCalculationService : ISaldoCalculationService
{
    private IApartmentSaldoRepository _saldoRepository;

    public SaldoCalculationService(IApartmentSaldoRepository repository)
    {
        _saldoRepository = repository;
    }

    public async Task<ServiceResult<string>> CalculateNewOutcomingSaldoByIncoming(CreateApartmentSaldoDto saldo)
    {
        try
        {
            var lastCalculatedSaldo = await _saldoRepository.ReadLastSaldoByApartmentNumber(saldo.AppartmentNumber);
            if (lastCalculatedSaldo == null)
            {
                await _saldoRepository.CreateSaldo(
                    saldo.AppartmentNumber,
                    saldo.IncomingSaldo,
                    saldo.OutcomingSaldo,
                    saldo.Description);
            }
            else
            {
                const decimal charge = 3000;
                const decimal payment = 1500;

                decimal incomingSaldo = lastCalculatedSaldo.IncomingSaldo;
                decimal outcomingSaldo = incomingSaldo + charge - payment;

                await _saldoRepository.CreateSaldo(saldo.AppartmentNumber, incomingSaldo, outcomingSaldo,
                    saldo.Description);
            }
            
            return ServiceResult<string>.Success($"Сальдо на квартиру {saldo.AppartmentNumber} успешно создано");
            
        }
        catch (DbUpdateException ex)
        {
            return ServiceResult<string>.Failure(ex.Message);
        }
    }
}