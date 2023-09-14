
using System.Linq.Expressions;
using Dominio.Entities;

namespace Dominio.Interface;

public interface IGeneric<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity); 
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    IEnumerable<T> Find(Expression<Func<T, bool>> expression);
}
