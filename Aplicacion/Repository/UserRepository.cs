
using Dominio.Entities;
using Dominio.Interface;
using Persistencia;

namespace Aplicacion.Repository;

public class UserRepository : GenericRepository<User>, IUser
{
    public UserRepository(MainContext context) : base(context)
    {
    }
}
