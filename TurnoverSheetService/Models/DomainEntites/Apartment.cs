using TurnoverSheetService.Services;

namespace TurnoverSheetService.Models.DomainEntites;

public partial class Apartment
{
    public Guid ApartmentGuid { get; private set; }
    public int Number { get; private set; }
    
    public string Address { get; private set; } = null!;
    
    private readonly List<Saldo> _saldos = new();
    public IReadOnlyCollection<Saldo> Saldos => _saldos;
    
    private Apartment(int number, string address)
    {
        Number = number;
        Address = address;
    }

    public static ServiceResult<Apartment> CreateApartmentFactory(int number, string address)
    {
        if (number < 0)
            return ServiceResult<Apartment>.Failure("Номер квартиры не может быть отрицательным");
        if (string.IsNullOrEmpty(address))
            return ServiceResult<Apartment>.Failure("Адрес квартиры пуст");

        return ServiceResult<Apartment>.Success(new Apartment(number, address));
    }

    public ServiceResult<string> AddSaldo(string serviceType, decimal incomingSaldoValue)
    {
        DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
        DateOnly lastMonthDate = currentDate.AddMonths(-1);
        var lastMonthDateSaldo = _saldos.FirstOrDefault(saldo => saldo.ServiceType == serviceType &&
                                                            saldo.CalculatingDate.Month == lastMonthDate.Month &&
                                                            saldo.CalculatingDate.Year == lastMonthDate.Year);

        if (string.IsNullOrEmpty(serviceType))
            return ServiceResult<string>.Failure("Тип оказаной услуги пуст");
        if (incomingSaldoValue < 0)
            return ServiceResult<string>.Failure("Входящее сальдо должно быть положительным");

        Saldo newSaldo;
        if (lastMonthDateSaldo != null) 
            newSaldo = new Saldo(serviceType, lastMonthDateSaldo.CurrentSaldoValue, ApartmentGuid);
        else
            newSaldo = new Saldo(serviceType, incomingSaldoValue, ApartmentGuid);
        
        _saldos.Add(newSaldo);
        return ServiceResult<string>.Success("Сальдо для квартиры успешно создано");
    }

    public ServiceResult<string> RemoveSaldo(string serviceType, DateOnly calculatingDate)
    {
        var existingSaldo = _saldos.FirstOrDefault(saldo => saldo.ServiceType == serviceType &&
                                                            saldo.CalculatingDate == calculatingDate);
        if (existingSaldo == null)
            return ServiceResult<string>.Failure("Сальдо по квартире на данную услугу не найдено");
        _saldos.Remove(existingSaldo);
        return ServiceResult<string>.Success("Сальдо по квартире удалено");
    }

    private ServiceResult<string> CreateTurnoverForSaldo(string saldoServiceType,
        DateOnly saldoCalculatingDate, Func<Saldo, ServiceResult<string>> saldoOperation)
    {
        var existingSaldo = _saldos.FirstOrDefault(saldo => saldo.ServiceType == saldoServiceType &&
                                                            saldo.CalculatingDate == saldoCalculatingDate);
        if (existingSaldo == null)
            return ServiceResult<string>.Failure("Сальдо не найдено по заданной услуге и дате");
        
        var turnoverAddingResult = saldoOperation(existingSaldo);
        if (!turnoverAddingResult.IsSuccess)
            return ServiceResult<string>.Failure(turnoverAddingResult.ErrorMessage);
        return ServiceResult<string>.Success(turnoverAddingResult.ResultObject);
    }

    public ServiceResult<string> CreateChargeForSaldo(decimal amount, string saldoServiceType, DateOnly saldoCalculatingDate)
    {
        return CreateTurnoverForSaldo(saldoServiceType, saldoCalculatingDate, saldo => saldo.AddCharge(amount));
    }

    public ServiceResult<string> RemoveChargeForSaldo(string serviceType, DateOnly calculatingDate, DateOnly chargeDate)
    {
        return CreateTurnoverForSaldo(serviceType, calculatingDate, saldo => saldo.RemoveCharge(chargeDate));
    }
    

    public ServiceResult<string> CreatePaymentForSaldo(decimal amount, string saldoServiceType, DateOnly saldoCalculatingDate)
    {
        return CreateTurnoverForSaldo(saldoServiceType, saldoCalculatingDate, saldo => saldo.AddPayment(amount));
    }

    public ServiceResult<string> RemovePaymentForSaldo(string serviceType, DateOnly calculatingDate, DateOnly paymentDate)
    {
        return CreateTurnoverForSaldo(serviceType, calculatingDate, saldo =>  saldo.RemovePayment(paymentDate));
    }
}