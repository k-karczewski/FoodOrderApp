using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.PizzaModels.PriceModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.Services
{
    public interface IFoodService<T> where T : class
    {
        /// <summary>
        /// Gets all T objects from database
        /// </summary>
        /// <returns>IServiceResult with operation status, List of T objects from database</returns>
        Task<IServiceResult<List<T>>> GetAsync();
        /// <summary>
        /// Gets T object with specific id
        /// </summary>
        /// <param name="id">id of object</param>
        /// <returns>IServiceResult with operation status, T objects from database</returns>
        Task<IServiceResult<T>> GetByIdAsync(int id);
        /// <summary>
        /// Creates new object in database
        /// </summary>
        /// <param name="newObject">data of object to be created</param>
        /// <returns>IServiceResult with operation status</returns>
        Task<IServiceResult<T>> CreateAsync(T newObject);
        /// <summary>
        /// Updates price of existing object
        /// </summary>
        /// <param name="newObject">new object data</param>
        /// <param name="id">id of object to be updated</param>
        /// <param name="size">size the price is related with</param>
        /// <returns>IServiceResult with operation status</returns>
        Task<IServiceResult<T>> UpdatePriceAsync(PriceModel newObject, int id);
        /// <summary>
        /// Removes object from databse
        /// </summary>
        /// <param name="objectId">id of object to be removed</param>
        /// <returns>IServiceResult with operation status</returns>
        Task<IServiceResult> DeleteAsync(int objectId);
    }
}
