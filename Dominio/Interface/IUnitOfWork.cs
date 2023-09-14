
namespace Dominio.Interface;

public interface IUnitOfWork
{
    IUser Users { get; }
    IRol Rols { get; }
    IRefreshToken RefreshTokens { get; }
    Task<int> SaveAsync();
}
