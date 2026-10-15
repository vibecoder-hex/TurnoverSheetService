using Microsoft.AspNetCore.Mvc;
using TurnoverSheetService.Models.Presentation;
using TurnoverSheetService.Services.DomainServices;

namespace TurnoverSheetService.Controllers.DomainControllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentManagementService _paymentService;
    
    public PaymentController(IPaymentManagementService paymentService)
    {
        _paymentService = paymentService; 
    }
    
    [HttpPost]
    public async Task<IActionResult> Post(CreatePaymentRequestDto dto)
    {
        var creationResult = await _paymentService.HandleCreatePayment(dto);
        if (!creationResult.IsSuccess)
            return BadRequest(new { Error = creationResult.ErrorMessage });
        return Ok(new { Message = creationResult.ResultObject });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeletePaymentRequestDto dto)
    {
        var deletionResult = await _paymentService.HandleDeletePayment(dto);
        if (!deletionResult.IsSuccess)
            return BadRequest(new { Message = deletionResult.ErrorMessage });
        return Ok(new { Message = deletionResult.ResultObject });
    }
}