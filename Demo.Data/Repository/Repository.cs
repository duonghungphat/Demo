using Demo.Data;
using Demo.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Demo.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public IQueryable<T> BuildQuery(Expression<Func<T, bool>> predicate)
        {
            return _dbSet
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Where(predicate);
        }
        public IQueryable<T> BuildQueryIncludingDeleted(Expression<Func<T, bool>> predicate)
        {
            return _dbSet
                .Where(predicate);
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            entity.IsDeleted = true;

            _dbSet.Update(entity);
        }
    }
}