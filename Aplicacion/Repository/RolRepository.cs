
using Dominio.Entities;
using Dominio.Interface;
using Persistencia;

namespace Aplicacion.Repository;

public class RolRepository : GenericRepository<Rol>, IRol
{
    public RolRepository(MainContext context) : base(context)
    {
    }
}
