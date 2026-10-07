using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Models;
using TurnoverSheetService.Repositories;

namespace TurnoverSheetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChargesOnPersonalAccountController : ControllerBase
{
    private readonly IApartmentChargesRepository _chargesRepository;
    public ChargesOnPersonalAccountController(IApartmentChargesRepository chargesRepository)
    {
        _chargesRepository = chargesRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateApartmentChargeDto dto)
    {
        try
        {
            await _chargesRepository.CreateCharge(dto.ApartmentNumber, dto.Amount, dto.Description);
            return Ok(new { Message = $"Начисление с наименованием {dto.Description} успешно добавлено" });
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}