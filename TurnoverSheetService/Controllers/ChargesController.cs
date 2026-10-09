using Microsoft.AspNetCore.Mvc;
using TurnoverSheetService.Models.Presentation;

namespace TurnoverSheetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChargesController : ControllerBase
{
    public ChargesController()
    {
        
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateChargeControllerDto dto)
    {
        return Ok();
    }
}