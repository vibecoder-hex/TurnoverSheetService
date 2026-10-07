using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Models;
using TurnoverSheetService.Repositories;

namespace TurnoverSheetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsFromPersonalAccountController : ControllerBase
{
    private readonly IApartmentPaymentsRepository _paymentsRepository;
    
    public PaymentsFromPersonalAccountController(IApartmentPaymentsRepository paymentsRepository)
    {
        _paymentsRepository = paymentsRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateApartmentPaymentDto dto)
    {
        try
        {
            await _paymentsRepository.CreatePayment(dto.ApartmentNumber, dto.Amount, dto.Description);
            return Ok(new { Message = $"Платеж на квартиру {dto.ApartmentNumber} успешно создан"});
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
}