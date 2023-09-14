
namespace DinoApi.Dtos.Estructure;

public class ResultLoginDto
{
    public string Username { get; set; }
    public string Token { get; set; }
    public string RefreshToken{ get; set; }
    public string Message { get; set; }
}
