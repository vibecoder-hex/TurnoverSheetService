using Microsoft.AspNetCore.Mvc;
using TurnoverSheetService.Models.Presentation;
using TurnoverSheetService.Services.DomainServices;

namespace TurnoverSheetService.Controllers.DomainControllers;

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
    public async Task<ActionResult> Post([FromBody] CreateApartmentRequestDto dto)
    {
        var creationResult = await _apartmentService.HandleCreateApartment(dto);
        if (creationResult.IsSuccess)
            return Ok(new { Message = creationResult.ResultObject });
        return BadRequest(new { Error = creationResult.ErrorMessage });
    }

    [HttpGet]
    public async Task<IActionResult> GetApartmentWithTurnoverDetails([FromQuery] int number, [FromQuery] string address)
    {
        var gettingResult = await _apartmentService.GetApartmentWithTurnoverDetails(number, address);
        if (gettingResult.IsSuccess)
            return Ok(gettingResult.ResultObject);
        return BadRequest(new { Error = gettingResult.ErrorMessage });
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteApartment([FromQuery] int number, [FromQuery] string address)
    {
        var deleteResult = await _apartmentService.DeleteApartment(number, address);
        if (deleteResult.IsSuccess)
            return Ok(new { Message = deleteResult.ResultObject });
        return BadRequest(new { Error = deleteResult.ErrorMessage });
    }
}   