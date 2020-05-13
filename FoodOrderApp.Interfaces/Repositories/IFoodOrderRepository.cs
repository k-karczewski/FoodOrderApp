using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.Repositories
{
    public interface IFoodOrderRepository<TEntity> where TEntity : class
    {
        Task<bool> CreateAsync(TEntity newObject);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> selector = null, 
                                           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);

    }
}
