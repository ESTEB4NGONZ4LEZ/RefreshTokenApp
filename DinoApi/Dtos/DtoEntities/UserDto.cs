
using System.ComponentModel.DataAnnotations;

namespace DinoApi.Dtos.DtoEntities;

public class UserDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
