using Ecom.Core.Interfaces;
using Ecom.infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Repositries
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            //throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
            }
            await _context.SaveChangesAsync();
            //throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
            //throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] include)
        {
            var query = _context.Set<T>().AsQueryable();
            foreach (var includeProperty in include)
            {
                query = query.Include(includeProperty);
            }
            
            return Task.FromResult(query.AsNoTracking().AsEnumerable());

            //throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(int id)
        {
            var entity =  _context.Set<T>().FindAsync(id);
            
            if(entity==null)
            {
                return null;
            }

            return entity.AsTask();

            //throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] include)
        {
            var query = _context.Set<T>().AsQueryable();
            foreach(var includeProperty in include)
            {
                query = query.Include(includeProperty);
            }
            return Task.FromResult(query.FirstOrDefault(e => EF.Property<int>(e, "Id") == id)!);
            //throw new NotImplementedException();
        }

        public async Task<int> GetCountAsync()
        {
           return await _context.Set<T>().CountAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();

            //----------------------

            //_context.Entry(entity).State = EntityState.Modified;
            //await _context.SaveChangesAsync();
            
            //throw new NotImplementedException();
        }
    }
}
