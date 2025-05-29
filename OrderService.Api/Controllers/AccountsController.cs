using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Abstractions;
using OrderService.Application.Models.Authentication.Const;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAuthService _authService;

    public AccountsController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(UserLoginDto userLoginDto) 
    {
        var result = await _authService.Login(userLoginDto);

        return Ok(result);
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
    {
        var result = await _authService.Register(userRegisterDto);
        return Ok(result);
    }
}