
using Aplicacion.Repository;
using Dominio.Interface;
using Persistencia;

namespace Aplicacion.UnitOfWork;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly MainContext _context;
    public UnitOfWork(MainContext context)
    {
        _context = context;
    }
    private UserRepository _users;
    private RolRepository _rols;
    private RefreshTokenRepository _refreshTokens;

    public IUser Users 
    {
        get
        {
            _users ??= new UserRepository(_context);
            return _users;
        }
    }

    public IRol Rols
    {
        get
        {
            _rols ??= new RolRepository(_context);
            return _rols;
        }
    }

    public IRefreshToken RefreshTokens 
    {
        get
        {
            _refreshTokens ??= new RefreshTokenRepository(_context);
            return _refreshTokens;
        }
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
