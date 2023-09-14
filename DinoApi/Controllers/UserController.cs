
using DinoApi.Dtos.DtoEntities;
using DinoApi.Dtos.Estructure;
using DinoApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace DinoApi.Controllers;

public class UserController : BaseApiController
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpPost("register")]
    public async Task<ActionResult<RegisterDto>> RegisterUserAsync(UserDto dataUser)
    {
        var user = await _userService.RegisterUser(dataUser);
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ResultLoginDto>> LoginAsync(LoginDto dataUser)
    {
        var login = await _userService.Login(dataUser);
        return login;
    }

    [HttpPost("refreshToken")]
    public async Task<ActionResult<ResultLoginDto>> RefreshToken(ValidateRefreshTokenDto tokens)
    {
        var jwtTokens = await _userService.ValidateRefreshToken(tokens);
        return jwtTokens;
    }
}
