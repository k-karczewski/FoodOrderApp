using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.DetailModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.Services
{
    public interface IIngredientService : IFoodService
    {
        /// <summary>
        /// Creates new object in database
        /// </summary>
        /// <param name="newIngredient">data of object to be created</param>
        /// <returns>IServiceResult with operation status</returns>
        Task<IServiceResult<IngredientModel>> CreateAsync(IngredientModel newIngredient);

        /// <summary>
        /// Gets all T objects from database
        /// </summary>
        /// <returns>IServiceResult with operation status, List of T objects from database</returns>
        Task<IServiceResult<List<IngredientModel>>> GetAsync();

        /// <summary>
        /// Updates price of existing ingredient
        /// </summary>
        /// <param name="price">new object data</param>
        /// <param name="ingredientId">id of ingredient which price will be updated</param>
        /// <returns>Updated ingredient or list of errors</returns>
        Task<IServiceResult<IngredientModel>> UpdatePriceAsync(IngredientDetailsToCreateDto price, int ingredientId);

        /// <summary>
        /// Gets Ingredient object with specific id
        /// </summary>
        /// <param name="id">id of ingredient</param>
        /// <returns>IServiceResult with operation status, T objects from database</returns>
        Task<IServiceResult<IngredientModel>> GetByIdAsync(int id);
    }
}
