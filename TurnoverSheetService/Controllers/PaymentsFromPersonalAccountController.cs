using Microsoft.AspNetCore.Mvc;

namespace TurnoverSheetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsFromPersonalAccountController : ControllerBase
{
    public PaymentsFromPersonalAccountController()
    {
        
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}