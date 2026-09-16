using Demo.Models;
using System.Linq.Expressions;

namespace Demo.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> BuildQuery(Expression<Func<T, bool>> predicate);

        IQueryable<T> BuildQueryIncludingDeleted(Expression<Func<T, bool>> predicate);

        Task<T?> GetByIdAsync(int id);

        Task<bool> ExistsAsync(int id);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}