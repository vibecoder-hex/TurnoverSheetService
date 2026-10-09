using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Models.DatabaseEntites;
using TurnoverSheetService.Models.Domain;
using TurnoverSheetService.Repositories;

namespace TurnoverSheetService.Services.DomainServices;

public interface IApartmentManagementService
{
    Task<ServiceResult<string>> CreateNewApartment(CreateApartmentDomainDto apartmentDto);
}

public class ApartmentManagementService : IApartmentManagementService
{
    private readonly IApartmentsRepository _apartmentsRepository;

    public ApartmentManagementService(IApartmentsRepository apartmentsRepository)
    {
        _apartmentsRepository = apartmentsRepository;
    }

    public async Task<ServiceResult<string>> CreateNewApartment(CreateApartmentDomainDto apartmentDto)
    {
        try
        {
            Guid existingApartment = await _apartmentsRepository.ReadApartmentByNumber(apartmentDto.Number);
            if (existingApartment != Guid.Empty)
                return ServiceResult<string>.Failure("Квартира с заданным номером существует");

            var newApartment = new Apartment
            {
                Number = apartmentDto.Number,
                Address = apartmentDto.Address
            };
            await _apartmentsRepository.CreateApartment(newApartment);
            return ServiceResult<string>.Success("Квартира успешно добавлена в реестр");
        }
        catch (DbUpdateException ex)
        {
            return ServiceResult<string>.Failure(ex.Message);
        }
    }
}