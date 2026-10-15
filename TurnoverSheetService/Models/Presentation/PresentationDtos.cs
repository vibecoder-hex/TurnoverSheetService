namespace TurnoverSheetService.Models.Presentation;

//ApartmentDtos
public record CreateApartmentRequestDto(int Number, string Address);
public record ReadApartmentResponseDto(int Number, string Address, List<ReadSaldoResponseDto> Saldos);

//SaldoDtos
public record CreateSaldoRequestDto(string ServiceType, decimal IncomingSaldoValue, int ApartmentNumber, string ApartmentAddress);
public record DeleteSaldoRequestDto(string ServiceType, DateOnly CalculatingDate, int ApartmentNumber, string ApartmentAddress);

public record ReadSaldoResponseDto(
    string ServiceType,
    decimal OutcomingSaldoValue,
    DateOnly CalculatingDate,
    List<ReadChargeResponseDto> Charges,
    List<ReadPaymentResponseDto> Payments);

//ChargeDtos
public record CreateChargeRequestDto(
    decimal Amount,
    string SaldoServiceType,
    DateOnly SaldoCalculatingDate, 
    int ApartmentNumber,
    string ApartmentAddress);

public record DeleteChargeRequestDto(
    DateOnly ChargeDate,
    string SaldoServiceType,
    DateOnly SaldoCalculatingDate,
    int ApartmentNumber,
    string ApartmentAddress);

public record ReadChargeResponseDto(decimal Amount, DateOnly ChargeDate);

//PaymentDtos
public record CreatePaymentRequestDto(
    decimal Amount,
    string SaldoServiceType,
    DateOnly SaldoCalculatingDate,
    int ApartmentNumber,
    string ApartmentAddress);

public record ReadPaymentResponseDto(decimal Amount, DateOnly PaymentDate);

public record DeletePaymentRequestDto(
    DateOnly PaymentDate,
    string SaldoServiceType,
    DateOnly SaldoCalculatingDate,
    int ApartmentNumber,
    string ApartmentAddress);

