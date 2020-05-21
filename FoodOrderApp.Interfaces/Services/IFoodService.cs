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
        /// Gets T object with specific id
        /// </summary>
        /// <param name="id">id of object</param>
        /// <returns>IServiceResult with operation status, T objects from database</returns>
        Task<IServiceResult<T>> GetByIdAsync(int id);

        /// <summary>
        /// Removes object from databse
        /// </summary>
        /// <param name="objectId">id of object to be removed</param>
        /// <returns>Operation status (Deleted or Error)</returns>
        Task<IServiceResult> DeleteAsync(int objectId);
    }
}
