using Microsoft.AspNetCore.Mvc;
using TurnoverSheetService.Models.Domain;
using TurnoverSheetService.Models.Presentation;
using TurnoverSheetService.Services.DomainServices;

namespace TurnoverSheetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApartmentController : ControllerBase
{
    private readonly IApartmentManagementService _apartmentService;

    public ApartmentController(IApartmentManagementService apartmentService)
    {
        _apartmentService = apartmentService;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateApartmentControllerDto dto)
    {
        var creationResult = await _apartmentService.CreateNewApartment(new CreateApartmentDomainDto(dto.Number, dto.Address));
        if (creationResult.IsSuccess)
            return Ok(new { Message = creationResult.ResultObject });
        return BadRequest(new { Error = creationResult.ErrorMessage });
    }
}    