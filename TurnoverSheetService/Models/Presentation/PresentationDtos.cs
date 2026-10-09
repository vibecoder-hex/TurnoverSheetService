namespace TurnoverSheetService.Models.Presentation;

//ApartmentDtos
public record CreateApartmentControllerDto(int Number, string Address);
public record ReadApartmentControllerDto(int Number, string Address);


// Saldo dtos
public record CreateSaldoControllerDto(int ApartmentNumber, decimal CurrentSaldoValue, DateOnly CalculatingDate, string Description);
public record ReadSaldoControllerDto(
    DateOnly CalculationDate,
    decimal CurrentSaldoValue,
    string Description,
    decimal TotalCharge,
    decimal TotalPayment);

// Charge dtos    
public record CreateChargeControllerDto(decimal Amount, string Description);
public record ReadChargeControllerDto(decimal Amount, string Description);

// Payment dtos
public record CreatePaymentControllerDto(decimal Amount, string Description);
public record ReadPaymentControllerDto(decimal Amount, string Description);