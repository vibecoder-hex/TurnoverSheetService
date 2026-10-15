using EntityFramework.Exceptions.Common;
using TurnoverSheetService.Models.Presentation;
using TurnoverSheetService.Models.DomainEntites;
using TurnoverSheetService.Repositories;

namespace TurnoverSheetService.Services.DomainServices;

public interface IChargeManagementService
{
    Task<ServiceResult<string>> HandleCreateCharge(CreateChargeRequestDto createDto);
    Task<ServiceResult<string>> HandleDeleteCharge(DeleteChargeRequestDto deleteDto);
}

public interface IPaymentManagementService
{
    Task<ServiceResult<string>> HandleCreatePayment(CreatePaymentRequestDto createDto);
    Task<ServiceResult<string>> HandleDeletePayment(DeletePaymentRequestDto deleteDto);
}

public abstract class TurnoverManagementServiceBase
{
    private readonly IRootAggregateCommandRepository _commandRepository;
    
    public TurnoverManagementServiceBase(IRootAggregateCommandRepository commandRepository)
    {
        _commandRepository = commandRepository;
    }
    
    protected async Task<ServiceResult<string>> HandleTurnoverOperation(
        int apartmentNumber, string apartmentAddress,
        Func<Apartment, ServiceResult<string>> apartmentOperation
        )
    {
        var apartment = await _commandRepository
            .GetApartmentEntityForExtensions(apartmentNumber, apartmentAddress);
        if (apartment == null)
            return ServiceResult<string>.Failure("Квартира не найдена по данному адресу и номеру");
        

        var chargeAddingResult = apartmentOperation(apartment);
        if (!chargeAddingResult.IsSuccess)
            return ServiceResult<string>.Failure(chargeAddingResult.ErrorMessage);

        await _commandRepository.SaveChangesAsync();
        return ServiceResult<string>.Success(chargeAddingResult.ResultObject);
    }
}

public class ChargeManagementService : TurnoverManagementServiceBase, IChargeManagementService
{

    public ChargeManagementService(IRootAggregateCommandRepository commandRepository) : base(commandRepository)
    {
        
    }

    public async Task<ServiceResult<string>> HandleCreateCharge(CreateChargeRequestDto createDto)
    {
        try
        {
            return await HandleTurnoverOperation(
                createDto.ApartmentNumber,
                createDto.ApartmentAddress,
                apartment => apartment.CreateChargeForSaldo(
                    createDto.Amount,
                    createDto.SaldoServiceType,
                    createDto.SaldoCalculatingDate
                    ));
        }
        catch (UniqueConstraintException)
        {
            return ServiceResult<string>.Failure("Начисление по квартире уже существует за этот день");
        }
    }

    public async Task<ServiceResult<string>> HandleDeleteCharge(DeleteChargeRequestDto deleteDto)
    {
        return await HandleTurnoverOperation(
            deleteDto.ApartmentNumber, 
            deleteDto.ApartmentAddress, 
            apartment => apartment.RemoveChargeForSaldo(
                deleteDto.SaldoServiceType,
                deleteDto.SaldoCalculatingDate,
                deleteDto.ChargeDate)
            );
    }
}

public class PaymentManagementService : TurnoverManagementServiceBase, IPaymentManagementService
{
    public PaymentManagementService(IRootAggregateCommandRepository commandRepository) : base(commandRepository)
    {
        
    }

    public async Task<ServiceResult<string>> HandleCreatePayment(CreatePaymentRequestDto createDto)
    {
        return await HandleTurnoverOperation(
            createDto.ApartmentNumber,
            createDto.ApartmentAddress,
            apartment => apartment.CreatePaymentForSaldo(
                createDto.Amount,
                createDto.SaldoServiceType,
                createDto.SaldoCalculatingDate
                )
        );
    }

    public async Task<ServiceResult<string>> HandleDeletePayment(DeletePaymentRequestDto deleteDto)
    {
        return await HandleTurnoverOperation(
            deleteDto.ApartmentNumber,
            deleteDto.ApartmentAddress,
            apartment => apartment.RemovePaymentForSaldo(
                deleteDto.SaldoServiceType,
                deleteDto.SaldoCalculatingDate,
                deleteDto.PaymentDate));
    }
}