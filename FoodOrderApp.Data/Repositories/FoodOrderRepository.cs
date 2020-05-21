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
        private readonly DbSet<TEntity> _dbSet;

        public FoodOrderRepository(FoodOrderContext context)
        {
            _dbSet = context.Set<TEntity>();
        }

        public async Task<bool> CreateAsync(TEntity newObject)
        {
            var entity = _dbSet.Attach(newObject);
            await _dbSet.AddAsync(newObject);
           
            if (entity.State == EntityState.Added)
            {
                return true;
            }
            else
            {
                throw new Exception("Object cannot be created");
            }
        }

        public async Task<bool> DeleteAsync(TEntity objectToDelete)
        {

            if (objectToDelete != null)
            {
               _dbSet.Remove(objectToDelete);
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
            _dbSet.Update(editedObject);

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
    }
}
