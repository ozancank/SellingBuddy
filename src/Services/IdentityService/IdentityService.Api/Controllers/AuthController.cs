using IdentityService.Api.Application.Models;
using IdentityService.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : Controller
{
    private readonly IIdentityService _ıdentityService;

    public AuthController(IIdentityService ıdentityService)
    {
        _ıdentityService = ıdentityService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequestModel)
    {
        var result = await _ıdentityService.Login(loginRequestModel);

        return Ok(result);
    }
}