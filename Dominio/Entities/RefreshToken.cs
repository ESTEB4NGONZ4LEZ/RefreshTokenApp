
namespace Dominio.Entities;

public class RefreshToken : BaseEntity
{
    public int IdUser { get; set; }
    public string Token { get; set; }
    public DateTime AddedDate { get; set; } = DateTime.Now;
    public DateTime ExpiryDate { get; set; }
}
