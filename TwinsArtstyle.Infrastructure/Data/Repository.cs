﻿using Microsoft.EntityFrameworkCore;
using TwinsArtstyle.Infrastructure.Interfaces;

namespace TwinsArtstyle.Infrastructure.Data
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext dbContext;

        public Repository(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public async Task Add<T>(T entity) where T : class
        {
            await DbSet<T>().AddAsync(entity);
        }

        public async Task AddRange<T>(ICollection<T> entities) where T : class
        {
            await DbSet<T>().AddRangeAsync(entities);
        }

        public IQueryable<T> All<T>() where T : class
        {
            return DbSet<T>();
        }

        private DbSet<T> DbSet<T>() where T : class
        {
            return dbContext.Set<T>();
        }

        public async Task<int> SaveChanges()
        {
            return await dbContext.SaveChangesAsync();
        }

        public void Remove<T>(T entity) where T : class
        {
            dbContext.Remove(entity);
        }

        public void RemoveRange<T>(IEnumerable<T> entities) where T : class
        {
            dbContext.RemoveRange(entities);
        }

        public async Task<T> FindById<T>(object id) where T : class
        {
            var model = await DbSet<T>().FindAsync(id);
            
            return model;
        }
    }
}
