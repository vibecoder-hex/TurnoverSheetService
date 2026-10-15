using EntityFramework.Exceptions.Common;
using TurnoverSheetService.Models.DomainEntites;
using TurnoverSheetService.Models.Presentation;
using TurnoverSheetService.Repositories;

namespace TurnoverSheetService.Services.DomainServices;

public interface ISaldoManagementService
{
    Task<ServiceResult<string>> HandleCreateSaldo(CreateSaldoRequestDto createSaldoRequestDto);
    Task<ServiceResult<string>> HandleDeleteSaldo(DeleteSaldoRequestDto deleteSaldoRequestDto);
}
public class SaldoManagementService : ISaldoManagementService
{
    private readonly IRootAggregateCommandRepository _commandRepository;
    
    public SaldoManagementService(IRootAggregateCommandRepository commandRepository)
    {
        _commandRepository = commandRepository;
    }

    public async Task<ServiceResult<string>> HandleCreateSaldo(CreateSaldoRequestDto createDto)
    {
        try
        {
            return await HandleSaldoOperation(
                createDto.ApartmentNumber,
                createDto.ApartmentAddress,
                apartment => apartment.AddSaldo(
                    createDto.ServiceType,
                    createDto.IncomingSaldoValue
                )
            );
        }
        catch (UniqueConstraintException)
        {
            return ServiceResult<string>.Failure($"Сальдо на указаный месяц в году по квартире уже существует");
        }

    }

    public async Task<ServiceResult<string>> HandleDeleteSaldo(DeleteSaldoRequestDto deleteDto)
    {
        return await HandleSaldoOperation(
            deleteDto.ApartmentNumber,
            deleteDto.ApartmentAddress,
            apartment => apartment.RemoveSaldo(deleteDto.ServiceType, deleteDto.CalculatingDate)
        );
    }
 
    private async Task<ServiceResult<string>> HandleSaldoOperation(int apartmentNumber, string apartmentAddress, Func<Apartment, ServiceResult<string>> apartmentOperation)
    {
        var apartmentEntity = await _commandRepository.GetApartmentEntityForExtensions(apartmentNumber, apartmentAddress); 
        if (apartmentEntity == null)
            return ServiceResult<string>.Failure("Квартира связанная с данным сальдо не найдена");
            
        var saldoAddingResult = apartmentOperation(apartmentEntity);
            
        if (!saldoAddingResult.IsSuccess)
            return ServiceResult<string>.Failure(saldoAddingResult.ErrorMessage);
            
        await _commandRepository.SaveChangesAsync();
        return ServiceResult<string>.Success(saldoAddingResult.ResultObject);
    }
    
}