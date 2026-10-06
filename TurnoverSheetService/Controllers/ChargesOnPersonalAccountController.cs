using Microsoft.AspNetCore.Mvc;

namespace TurnoverSheetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChargesOnPersonalAccountController : ControllerBase
{
    public ChargesOnPersonalAccountController()
    {
        
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}