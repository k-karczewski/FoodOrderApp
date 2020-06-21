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

        public async Task CreateAsync(TEntity newObject)
        {
            await _dbSet.AddAsync(newObject);
        }

        public void Delete(TEntity objectToDelete)
        {
            _dbSet.Remove(objectToDelete);
        }

        public void Update(TEntity editedObject)
        {
            _dbSet.Update(editedObject);
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
    }
}
