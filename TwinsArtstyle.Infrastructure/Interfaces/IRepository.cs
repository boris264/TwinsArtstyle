using Microsoft.EntityFrameworkCore;

namespace TwinsArtstyle.Infrastructure.Interfaces
{
    public interface IRepository
    {
        public IQueryable<T> All<T>() where T : class;

        public Task Add<T>(T entity) where T : class;

        public Task AddRange<T>(ICollection<T> entities) where T : class;

        public Task<int> SaveChanges();

        public Task Remove<T>(T entity) where T : class;

        public Task RemoveRange<T>(IEnumerable<T> entities) where T : class;
    }
}
