using Microsoft.AspNetCore.Mvc;
using TurnoverSheetService.Models.Domain;
using TurnoverSheetService.Models.Presentation;
using TurnoverSheetService.Services.DomainServices;

namespace TurnoverSheetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SaldoController : ControllerBase
{
    private readonly ISaldoCalculationService _saldoService;
    public SaldoController(ISaldoCalculationService saldoService)
    {
        _saldoService = saldoService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateSaldoControllerDto dto)
    {
        var serviceResult = await _saldoService.CreateNewSaldoOnFutureDate(new CreateSaldoDomainDto(
            dto.ApartmentNumber,
            dto.CurrentSaldoValue,
            dto.CalculatingDate,
            dto.Description));
        
        if (serviceResult.IsSuccess)
            return Ok(new { Message = serviceResult.ResultObject });
        return BadRequest(new { Error = serviceResult.ErrorMessage });
    }
}