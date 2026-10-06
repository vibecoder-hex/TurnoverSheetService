namespace TurnoverSheetService.Models;


public abstract record ApartmentSaldoDto(int AppartmentNumber, decimal IncomingSaldo, decimal OutcomingSaldo, string Description);

public record CreateApartmentSaldoDto(int AppartmentNumber, decimal IncomingSaldo, decimal OutcomingSaldo,  string Description)
    : ApartmentSaldoDto(AppartmentNumber, IncomingSaldo, OutcomingSaldo, Description);

public record ReadApartmentSaldoDto(int AppartmentNumber, DateOnly PaymentDate, decimal IncomingSaldo, decimal OutcomingSaldo, string Description) 
    : ApartmentSaldoDto(AppartmentNumber, IncomingSaldo, OutcomingSaldo, Description);