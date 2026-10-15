using Microsoft.AspNetCore.Mvc;
using TurnoverSheetService.Models.Presentation;
using TurnoverSheetService.Services.DomainServices;

namespace TurnoverSheetService.Controllers.DomainControllers;

[ApiController]
[Route("api/[controller]")]
public class SaldoController : ControllerBase
{
    private readonly ISaldoManagementService _saldoServive;

    public SaldoController(ISaldoManagementService saldoService)
    {
        _saldoServive = saldoService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateSaldoRequestDto dto)
    {
        var creationResult = await _saldoServive.HandleCreateSaldo(dto);
        if (creationResult.IsSuccess)
            return Ok(new { Message = creationResult.ResultObject });
        return BadRequest(new { Error = creationResult.ErrorMessage });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteSaldoRequestDto dto)
    {
        var deletionResult = await _saldoServive.HandleDeleteSaldo(dto);
        if (deletionResult.IsSuccess)
            return Ok(new { Message = deletionResult.ResultObject });
        return BadRequest(new { Error = deletionResult.ErrorMessage });
    }
}