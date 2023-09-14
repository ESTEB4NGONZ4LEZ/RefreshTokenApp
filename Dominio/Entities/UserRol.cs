
namespace Dominio.Entities;

public class UserRol
{
    public int IdUser { get; set; }
    public int IdRol { get; set; }
    public User User { get; set; }
    public Rol Rol { get; set; }
}
