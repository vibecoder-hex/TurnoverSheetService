using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Models;
using TurnoverSheetService.Repositories;
using TurnoverSheetService.Services;

namespace TurnoverSheetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonalAccountSaldoController : ControllerBase 
{
    private readonly ISaldoCalculationService _saldoService;
    private readonly IApartmentSaldoRepository _saldoRepository;
    public PersonalAccountSaldoController(ISaldoCalculationService saldoService, IApartmentSaldoRepository saldoRepository)
    {
        _saldoService = saldoService;
        _saldoRepository = saldoRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateApartmentSaldoDto dto)
    {
        var serviceResult = await _saldoService.CalculateNewSaldoOnFutureDate(dto);
        return Ok(serviceResult);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var saldos = await _saldoRepository.ReadAllSaldos();
            return Ok(saldos);
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}