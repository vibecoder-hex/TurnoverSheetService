using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Models.DomainEntites;
using TurnoverSheetService.Models.Presentation;
using TurnoverSheetService.Repositories;

namespace TurnoverSheetService.Services.DomainServices;

public interface IApartmentManagementService
{
    Task<ServiceResult<string>> HandleCreateApartment(CreateApartmentRequestDto dto);
    Task<ServiceResult<ReadApartmentResponseDto>> GetApartmentWithTurnoverDetails(int number, string address);
    Task<ServiceResult<string>> DeleteApartment(int number, string address);
}

public class ApartmentManagementService : IApartmentManagementService
{
    private readonly IRootAggregateCommandRepository _commandRepository;
    private readonly IRootAggregateQueryRepository _queryRepository;

    public ApartmentManagementService(IRootAggregateCommandRepository commandRepository, IRootAggregateQueryRepository queryRepository)
    {
       _commandRepository = commandRepository;
       _queryRepository = queryRepository;
    }

    public async Task<ServiceResult<string>> HandleCreateApartment(CreateApartmentRequestDto apartmentDto)
    {
        try
        {
            ServiceResult<Apartment> createApartmentFactory =
                Apartment.CreateApartmentFactory(apartmentDto.Number, apartmentDto.Address);

            if (!createApartmentFactory.IsSuccess)
                return ServiceResult<string>.Failure(createApartmentFactory.ErrorMessage);

            await _commandRepository.CreateApartment(createApartmentFactory.ResultObject);
            await _commandRepository.SaveChangesAsync();
            return ServiceResult<string>.Success("Квартира успешно добавлена");
        }
        catch (UniqueConstraintException ex)
        {
            return ServiceResult<string>.Failure($"Ограничение {ex.ConstraintName} нарушено");
        }
    }
    
    public async Task<ServiceResult<ReadApartmentResponseDto>> GetApartmentWithTurnoverDetails(int number, string address)
    {
        var apartment = await _queryRepository.GetApartmentWithTurnoverDetails(number, address); 
        if (apartment == null) 
            return ServiceResult<ReadApartmentResponseDto>.Failure("Квартира с заданным номером и адресом не найдена");
        
        return ServiceResult<ReadApartmentResponseDto>.Success(apartment);
    }

    public async Task<ServiceResult<string>> DeleteApartment(int number, string address)
    { 
        bool isApartmentExists = await _queryRepository.IsApartmentExists(number, address);
        if (!isApartmentExists) 
            return ServiceResult<string>.Failure("Квартира с заданным номером не найдена");
        
        await _commandRepository.DeleteApartment(number, address); 
        return ServiceResult<string>.Success("Квартира успешно удалена");
    }
}