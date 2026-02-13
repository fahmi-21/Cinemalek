using Microsoft.EntityFrameworkCore;

namespace Cinemalek.Repository
{
    public interface IRepository <T> where T : class
    {
         Task CreateAsync(T entity);
         void Edit(T entity);
         void Delete(T entity);
         Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? includes = null, bool tracked = true);
         Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression, bool tracked = true);
         Task<int> Commitasync();
         Task<bool> AnyAsync(Expression<Func<T, bool>> expression);

    }
}
