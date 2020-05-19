using FoodOrderApp.Data.DataContext;
using FoodOrderApp.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Data.Repositories
{
    public class FoodOrderRepository<TEntity> : IFoodOrderRepository<TEntity> where TEntity : class
    {
        private readonly FoodOrderContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public FoodOrderRepository(FoodOrderContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<bool> CreateAsync(TEntity newObject)
        {
            await _dbSet.AddAsync(newObject);
           
            if (_context.Entry(newObject).State == EntityState.Added)
            {
                await CommitChangesAsync();
                return true;
            }
            else
            {
                throw new Exception("Object cannot be created");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            TEntity objectToDelete = await GetByIdAsync(id);

            if (objectToDelete != null)
            {
                _dbSet.Remove(objectToDelete);
                await CommitChangesAsync();
                return true;
            }
            else
            {
                throw new Exception("Object cannot be deleted");
            } 
        }

        public async Task<IEnumerable<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> selector = null, 
                                                  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if(include != null)
            {
                query = include(query);
            }

            if(selector != null)
            {
                query = query.Where(selector);
            }

            return query;
        }

        public async Task<bool> UpdateAsync(TEntity editedObject)
        {
            _context.Set<TEntity>().Update(editedObject);
            await CommitChangesAsync();

            return true; 
        }


        private async Task<TEntity> GetByIdAsync(int id)
        {
            TEntity objectFromDb = await _dbSet.FindAsync(id);
            
            if(objectFromDb == null)
            {
                throw new Exception("Object has not been found");
            }

            return objectFromDb; 
        }

        private async Task<bool> CommitChangesAsync()
        {
            return await _context.SaveChangesAsync() != 0 ? true : false;
        }
    }
}
