using FoodOrderApp.Interfaces.Repositories;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.PizzaModels.PriceModels;
using FoodOrderApp.Services.ServiceResults;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Services
{
    public abstract class FoodServiceBase<T> : IFoodService<T> where T : class 
    {
        protected readonly IFoodOrderRepository<T> _repository;

        public FoodServiceBase(IFoodOrderRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<IServiceResult<T>> CreateAsync(T newObject)
        {
            try
            {
                await _repository.CreateAsync(newObject);

                return new ServiceResult<T>(ResultType.Created, newObject);
            }
            catch(Exception e)
            {
                return new ServiceResult<T>(ResultType.Error, new List<string>() { e.Message });
            }
        }

        public virtual async Task<IServiceResult> DeleteAsync(int objectId)
        {
            try
            { 
                await _repository.DeleteAsync(objectId);
                
                return new ServiceResult(ResultType.Deleted);
            }
            catch(Exception e)
            {
                return new ServiceResult(ResultType.Error, new List<string>() { e.Message });
            }
        }

        public abstract Task<IServiceResult<T>> UpdatePriceAsync(PriceModel newPrices, int id);
        public abstract Task<IServiceResult<List<T>>> GetAsync();
        public abstract Task<IServiceResult<T>> GetByIdAsync(int id);
    }
}
