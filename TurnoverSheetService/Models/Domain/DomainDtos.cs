namespace TurnoverSheetService.Models.Domain;

public record CreateApartmentDomainDto(int Number, string Address);
public record ReadApartmentDomainDto(Guid ApartmentGuid, int Number, string Address);

public record CreateSaldoDomainDto(int ApartmentNumber, decimal CurrentSaldoValue, DateOnly CalculatingDate, string Description);
public record ReadSaldoDomainDto(
    Guid SaldoUuid,
    DateOnly CalculationDate,
    decimal CurrentSaldoValue,
    string Description,
    decimal TotalCharge,
    decimal TotalPayment,
    Guid ApartmentGuid);
    
public record CreateChargeDomainDto(Guid SaldoGuid, decimal Amount, string Description);
public record ReadChargeDomainDto(Guid ChargeGuid, decimal Amount, string Description, Guid SaldoGuid);

public record CreatePaymentDomainDto(decimal Amount, string Description);
public record ReadPaymentDomainDto(Guid SaldoGuid, decimal Amount, string Description, Guid PaymentGuid);