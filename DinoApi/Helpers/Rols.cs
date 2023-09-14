
namespace DinoApi.Helpers;

public class Rols
{
    public enum ListRols
    {
        Manager,
        Admin,
        Employee
    }
    public const ListRols defaultRol = ListRols.Employee;
}
