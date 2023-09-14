
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DinoApi.Dtos.DtoEntities;
using DinoApi.Dtos.Estructure;
using DinoApi.Helpers;
using Dominio.Entities;
using Dominio.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DinoApi.Service;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly Jwt _jwt;
    public UserService(
        IUnitOfWork unitOfWork, 
        IPasswordHasher<User> passwordHasher, 
        IOptions<Jwt> jwt
    )
    {
        _unitOfWork     = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwt            = jwt.Value;
    }
    public async Task<RegisterDto> RegisterUser(UserDto dataUser)
    {
        RegisterDto result = new();

        if(VerifyUserExists(dataUser.Username))
        {
            User user = new()
            {
                Username = dataUser.Username,
                Email = dataUser.Email  
            }; 

            user.Password = _passwordHasher.HashPassword(user, dataUser.Password);

            var rol = _unitOfWork.Rols
                                 .Find(x => x.Nombre == Rols.defaultRol.ToString())
                                 .First();
            
            user.Rols.Add(rol);

            try
            {
                _unitOfWork.Users.Add(user);
                await _unitOfWork.SaveAsync();
                result.Username = user.Username;
                result.Email = user.Email;
                result.Message = $"User {user.Username} successfully registered";
                return result;
            }
            catch (Exception error)
            {
                result.Message = $"Error : {error}";
                return result;
            }
        }
        else
        {
            result.Message = $"User {dataUser.Username} is already registed";
            return result;
        }
    }
    public async Task<ResultLoginDto> Login(LoginDto dataUser)
    {
        ResultLoginDto result = new();

        if(!VerifyUserExists(dataUser.Username))
        {
            var user = _unitOfWork.Users
                                  .Find(x => x.Username.ToLower() == dataUser.Username.ToLower())
                                  .FirstOrDefault();
            
            var passwordVerification = _passwordHasher.VerifyHashedPassword(user, user.Password, dataUser.Password);

            if(passwordVerification == PasswordVerificationResult.Success)
            {
                var refreshToken = GenerateRefreshToken(user.Id);
                _unitOfWork.RefreshTokens.Add(refreshToken);
                await _unitOfWork.SaveAsync();

                result.Username = user.Username;
                result.Token = CreateToken(user);
                result.RefreshToken = refreshToken.Token;
                result.Message = $"Token created successfully";

                return result;
            }
            else 
            {
                result.Username = dataUser.Username;
                result.Token = null;
                result.RefreshToken = null;
                result.Message = $"Incorrect Credentials";
                return result;
            }
        }
        else
        {
            result.Username = dataUser.Username;
            result.Token = null;
            result.RefreshToken = null;
            result.Message = $"The user {dataUser.Username} is not registered";
            return result;
        }
    }
    public async Task<ResultLoginDto> ValidateRefreshToken(ValidateRefreshTokenDto tokens)
    {
        var dbRefreshToken = _unitOfWork.RefreshTokens
                                        .Find(x => x.Token == tokens.RefreshToken)
                                        .FirstOrDefault();

        ResultLoginDto result = new();

        if(dbRefreshToken == null)
        {
            result.Message = "Invalid Parameters";
            return result;
        }

        var dbUser = _unitOfWork.Users 
                                .Find(x => x.Id == dbRefreshToken.IdUser)
                                .FirstOrDefault();

        var jwtToken = tokens.Token;

        // if(ValidateToken(jwtToken))
        // {
        //     result.Message = "Invalid Token";
        //     return result;
        // }

        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);

        string idUser = token.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value;

        if(!(idUser == dbUser.Id.ToString()))
        {
            result.Message = "Invalid Parameters";
            return result;
        }

        string username = token.Claims.FirstOrDefault(claim => claim.Type == "Username")?.Value;

        if(!(username == dbUser.Username))
        {
            result.Message = "Invalid Parameters";
            return result;
        }

        string email = token.Claims.FirstOrDefault(claim => claim.Type == "Email")?.Value;

        if(!(email == dbUser.Email))
        {
            result.Message = "Invalid Parameters";
            return result;
        }

        if(dbRefreshToken.ExpiryDate < DateTime.Now)
        {
            result.Message = "Invalid Expired";
            return result;
        }

        var refreshToken = GenerateRefreshToken(dbUser.Id);
        _unitOfWork.RefreshTokens.Add(refreshToken);
        await _unitOfWork.SaveAsync();

        result.Username = dbUser.Username;
        result.Token = CreateToken(dbUser);
        result.RefreshToken = refreshToken.Token;
        result.Message = $"Token created successfully";

        return result;
    }
    // private bool ValidateToken(string token)
    // {
    //     try
    //     {
    //         var tokenHandler = new JwtSecurityTokenHandler();
    //         var validationParameters = new TokenValidationParameters
    //         {
    //             ValidateIssuer = true,
    //             ValidateAudience = true,
    //             ValidateLifetime = false,
    //             ValidateIssuerSigningKey = true,
    //             ValidIssuer = _jwt.Issuer,
    //             ValidAudience = _jwt.Audience,
    //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key))
    //         };

    //         SecurityToken validatedToken;
    //         tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

    //         return true;
    //     }
    //     catch (Exception ex)
    //     {
    //         return false;
    //     }
    // }
    private string CreateToken(User dataUser)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var byteKey = Encoding.UTF8.GetBytes(_jwt.Key);

        var rols = dataUser.Rols;

        var rolsClaim = new List<Claim>();

        foreach(var rol in rols)
        {
            rolsClaim.Add(new Claim("Rols", rol.Nombre));
        }
        
        var tokenDes = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new("Id", dataUser.Id.ToString()),
                new("Username", dataUser.Username),
                new("Email", dataUser.Email),
                
            }.Union(rolsClaim)),
            Issuer = _jwt.Issuer,
            Audience = _jwt.Audience,
            Expires = DateTime.UtcNow.AddSeconds(_jwt.TimeExpired),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(byteKey),
                                                            SecurityAlgorithms.HmacSha256Signature)
                                                            
        };
        var token = tokenHandler.CreateToken(tokenDes);
        
        return tokenHandler.WriteToken(token);
    }
    private RefreshToken GenerateRefreshToken(int idUser)
    {
        var refreshToken = new RefreshToken
        {
            IdUser = idUser,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiryDate = DateTime.Now.AddMinutes(1)
        };
        return refreshToken;  
    }
    private bool VerifyUserExists(string username)
    {
        var user = _unitOfWork.Users 
                              .Find(x => x.Username.ToLower() == username.ToLower())
                              .FirstOrDefault();
        return user == null;
    }
}