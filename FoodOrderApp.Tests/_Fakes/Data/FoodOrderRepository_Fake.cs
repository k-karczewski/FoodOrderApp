using FoodOrderApp.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Tests._Fakes.Data
{
    public class FoodOrderRepository_Fake<T> : IFoodOrderRepository<T> where T : class
    {
        private List<T> items;

        public FoodOrderRepository_Fake()
        {
            items = new List<T>();
        }

        public async Task<bool> CreateAsync(T newObject)
        {
            items.Add(newObject);
            return true;
        }

        public async Task<bool> DeleteAsync(T objectToDelete)
        {
            items.Remove(objectToDelete);
            return true;
        }

        public async Task<IEnumerable<T>> GetByExpressionAsync(Expression<Func<T, bool>> selector = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = items.AsQueryable();

            if (include != null)
            {
                query = include(query);
            }

            if (selector != null)
            {
                query = query.Where(selector);
            }

            return query;
        }

        public async Task<bool> UpdateAsync(T editedObject)
        {
            var itemToEdit = items.SingleOrDefault(x => x == editedObject);
            itemToEdit = editedObject;

            return true;
        }
    }
}
