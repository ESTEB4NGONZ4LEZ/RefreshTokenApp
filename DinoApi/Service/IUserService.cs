
using DinoApi.Dtos.DtoEntities;
using DinoApi.Dtos.Estructure;

namespace DinoApi.Service;

public interface IUserService
{
    Task<RegisterDto> RegisterUser(UserDto dataUser);
    Task<ResultLoginDto> Login(LoginDto dataUser);
    Task<ResultLoginDto> ValidateRefreshToken(ValidateRefreshTokenDto tokens);
}
