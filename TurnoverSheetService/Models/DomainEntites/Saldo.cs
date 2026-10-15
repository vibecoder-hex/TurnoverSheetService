using TurnoverSheetService.Services;

namespace TurnoverSheetService.Models.DomainEntites;

public partial class Saldo
{
    public Guid SaldoUuid { get; private set; }
    
    public string ServiceType { get; private set; } = null!;

    public Apartment Apartment { get; private set; } = null!;
    
    public Guid ApartmentUuid { get; private set; }

    public DateOnly CalculatingDate { get; private set; }

    public decimal CurrentSaldoValue { get; private set; }

    private readonly List<Charge> _charges = new();

    private readonly List<Payment> _payments = new();
    
    public IReadOnlyCollection<Charge> Charges => _charges;
    
    public IReadOnlyCollection<Payment> Payments => _payments;

    internal Saldo(string serviceType, decimal currentSaldoValue, Guid apartmentUuid)
    {
        ServiceType = serviceType;
        CurrentSaldoValue = currentSaldoValue;
        CalculatingDate = DateOnly.FromDateTime(DateTime.Now);
        ApartmentUuid = apartmentUuid;
    }

    private void RecalculateSaldoValue(decimal currentChargeValue, decimal currentPaymentValue)
    {
        decimal outcomingSaldo = CurrentSaldoValue + currentChargeValue - currentPaymentValue;
        CurrentSaldoValue = outcomingSaldo;
    }

    public ServiceResult<string> AddCharge(decimal amount)
    {
        if (amount <= 0)
            return ServiceResult<string>.Failure("Значение начисления должно быть больше 0");

        var newCharge = new Charge(amount, SaldoUuid);
        _charges.Add(newCharge);
        RecalculateSaldoValue(amount, 0);
        return ServiceResult<string>.Success("Начисление по расчетному периоду успешно добавлено");
    }

    public ServiceResult<string> RemoveCharge(DateOnly chargeDate)
    {
        var existingCharge = _charges.FirstOrDefault(charge => charge.ChargeDate == chargeDate);
        if (existingCharge == null)
            return ServiceResult<string>.Failure("Начисление по заданному времени не найдено");
        
        _charges.Remove(existingCharge);
        RecalculateSaldoValue(-existingCharge.Amount, 0);
        return ServiceResult<string>.Success("Начисление по данному времени удалено");
    }

    public ServiceResult<string> AddPayment(decimal amount)
    {
        if (amount <= 0)
            return ServiceResult<string>.Failure("Значение платежа должно быть больше 0");

        var newPayment = new Payment(amount, SaldoUuid);
        _payments.Add(newPayment);
        RecalculateSaldoValue(0, amount);
        return ServiceResult<string>.Success("Платеж по расчетному периоду успешно добавлен");
    }

    public ServiceResult<string> RemovePayment(DateOnly paymentDate)
    {
        var existingPayment = _payments.FirstOrDefault(payment => payment.PaymentDate == paymentDate);
        if (existingPayment == null)
            return ServiceResult<string>.Failure("Платеж по заданному времени не найдено");
        
        _payments.Remove(existingPayment);
        RecalculateSaldoValue(0, -existingPayment.Amount);
        return ServiceResult<string>.Success("Платеж по данному времени удалено");
    }
}
