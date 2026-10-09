using Microsoft.AspNetCore.Mvc;
using TurnoverSheetService.Models.Domain;
using TurnoverSheetService.Models.Presentation;
using TurnoverSheetService.Services.DomainServices;

namespace TurnoverSheetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    public readonly IPaymentManagementService _paymentService;
    public PaymentsController(IPaymentManagementService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreatePaymentControllerDto dto)
    {
        var creationResult = await _paymentService.CreatePayment(new CreatePaymentDomainDto(dto.Amount, dto.Description));
        if (creationResult.IsSuccess)
            return Ok(creationResult.ResultObject);
        return BadRequest(creationResult.ErrorMessage);
    }
}