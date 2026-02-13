
using Microsoft.EntityFrameworkCore;

namespace Cinemalek.Repository
{
    public class Repositories <T> where T : class
    {
        protected AppDbContext _context = new();
        protected DbSet<T> _DbSet;
        public Repositories()
        {
            _DbSet =  _context.Set<T>();
        }
        // crud
        public async Task CreateAsync( T entity )
        {
          await _DbSet.AddAsync ( entity );
            
        }
        public void Edit (T entity )
        {
            _DbSet.Update ( entity );
            
        }
        public void Delete (T entity )
        {
            _DbSet.Remove ( entity );
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? includes = null , bool tracked = true )
        {
            var categories = _DbSet.AsQueryable ();

            //filter 
            if (expression is not null)
                categories = categories.Where(expression);
            if (!tracked)
                categories = categories.AsNoTracking ();
            if (includes is not null)
            {
                foreach ( var include in includes)
                {
                    if (include is not null)
                        categories = categories.Include(include);
                }

            }

            return await categories.ToListAsync();
        }
        public async Task<T?> GetOneAsync( Expression<Func<T, bool>>? expression ,bool tracked = true)
        {
            return (await GetAllAsync(expression: expression, tracked: tracked)) .FirstOrDefault();
        }

        public async Task<int> Commitasync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch ( Exception ex ) 
            {
                Console.WriteLine($"Error {ex.Message}");
                return 0;
            }
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().AnyAsync(expression);
        }


    }
}
