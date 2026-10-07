namespace TurnoverSheetService.Models;

// Saldo dtos
public record CreateApartmentSaldoDto(int ApartmentNumber, decimal CurrentSaldoValue, string Description);
public record ReadApartmentSaldoDto(int ApartmentNumber, DateOnly PaymentDate, decimal CurrentSaldoValue, string Description);

// Charge dtos    
public record CreateApartmentChargeDto(int ApartmentNumber,  decimal Amount, string Description);

public record ReadGroupedByFieldsApartmentChargeDto(int ApartmentNumber, int Month, decimal SumOfAmounts);
public record ReadApartmentChargeDto(int ApartmentNumber, DateOnly ChargeDate, decimal Amount, string Description);


// Payment dtos
public record CreateApartmentPaymentDto(int ApartmentNumber, decimal Amount, string Description);
public record ReadGroupedByFieldsApartmentPaymentsDto(int ApartmentNumber, int Month, decimal SumOfAmounts);
public record ReadApartmentPaymentDto(int ApartmentNumber, DateOnly PaymentDate, decimal Amount, string Description);