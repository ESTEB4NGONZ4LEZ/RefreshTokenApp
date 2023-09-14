
namespace DinoApi.Helpers;

public class Jwt
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public double TimeExpired { get; set; }
}
