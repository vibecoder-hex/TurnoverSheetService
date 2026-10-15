using Microsoft.AspNetCore.Mvc;
using TurnoverSheetService.Models.Presentation;
using TurnoverSheetService.Services.DomainServices;

namespace TurnoverSheetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChargeController : ControllerBase
{
    private readonly IChargeManagementService _chargeService;
    
    public ChargeController(IChargeManagementService chargeService)
    {
        _chargeService = chargeService;
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateChargeRequestDto dto)
    {
        var creationResult = await _chargeService.HandleCreateCharge(dto);
        if (!creationResult.IsSuccess)
            return BadRequest(new { Error = creationResult.ErrorMessage });
        return Ok(new { Message = creationResult.ResultObject });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteChargeRequestDto dto)
    {
        var creationResult = await _chargeService.HandleDeleteCharge(dto);
        if (!creationResult.IsSuccess)
            return BadRequest(new { Error = creationResult.ErrorMessage });
        
        return Ok(new { Message = creationResult.ResultObject });
    }
    
}