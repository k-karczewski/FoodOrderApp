﻿using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.Enums;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.DetailModels;
using FoodOrderApp.Services.ServiceResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderApp.Services
{
    public class IngredientService : FoodServiceBase, IIngredientService
    {
        public IngredientService(IUnitOfWork repository) : base(repository) { }

        /// <summary>
        /// Gets all ingredients from database with their prices
        /// </summary>
        /// <returns>List of all ingredients or errors (depending on result type)</returns>
        public async Task<IServiceResult<List<IngredientModel>>> GetAsync()
        {
            try
            {
                // try to get all ingredients
                List<IngredientModel> ingredients = (await _repository.Ingredients.GetByExpressionAsync(x => x.Id > 0, i => i.Include(id => id.IngredientDetails))).ToList();

                if(ingredients != null)
                {
                    return new ServiceResult<List<IngredientModel>>(ResultType.Correct, ingredients);
                }

                return new ServiceResult<List<IngredientModel>>(ResultType.Error, new List<string> { "Error during ingredient list get operation" });
            }
            catch (Exception e)
            {
                // catch errors and pass them to controller
                return new ServiceResult<List<IngredientModel>>(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Gets ingredient with id passed by parameter
        /// </summary>
        /// <param name="id">identifier of ingredient</param>
        /// <returns>Ingredient from database or list of errors (depending on result type)</returns>
        public async Task<IServiceResult<IngredientModel>> GetByIdAsync(int id)
        {
            try
            {
                // try to get ingredient with specific id
                IngredientModel result = (await _repository.Ingredients.GetByExpressionAsync(x => x.Id == id, i => i.Include(id => id.IngredientDetails))).SingleOrDefault();

                if (result != null)
                {
                    // return it to controller
                    return new ServiceResult<IngredientModel>(ResultType.Correct, result);
                }

                return new ServiceResult<IngredientModel>(ResultType.Error, new List<string>() { $"Object with id: {id} has not been found" });
            }
            catch (Exception e)
            {
                // catch errors and pass them to controller
                return new ServiceResult<IngredientModel>(ResultType.Error, new List<string>() { e.Message });
            }
        }

        /// <summary>
        /// Creates new ingredient
        /// </summary>
        /// <param name="newObject">ingredient to be added to database</param>
        /// <returns>Ingredient that has been added to database or errors (depending on result type)</returns>
        public async Task<IServiceResult<IngredientModel>> CreateAsync(IngredientModel newObject)
        {
            try
            {
                // check if ingredient with that name already exists in db
                bool doesExistInDb = (await _repository.Ingredients.GetByExpressionAsync(x => x.Name.ToLower() == newObject.Name.ToLower(), null)).SingleOrDefault() == null ? false : true;

                if(doesExistInDb == false)
                {    
                    // try to add ingredient to database
                    await _repository.Ingredients.CreateAsync(newObject);

                    // save changes in database context
                    await _repository.SaveChangesAsync();

                    // return Ok service result with created object
                    return new ServiceResult<IngredientModel>(ResultType.Created, newObject);                 
                }

                return new ServiceResult<IngredientModel>(ResultType.Error, new List<string> { $"Ingredient with name {newObject.Name} already exists in database" });
            }
            catch(Exception e)
            {
                // catch exception and pass errors to controller
                return new ServiceResult<IngredientModel>(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Deletes ingredient with id passed by parameter
        /// </summary>
        /// <param name="ingredientId">id of ingredient</param>
        /// <returns>Service Result with statuses: Deleted or Error</returns>
        public async Task<IServiceResult> DeleteAsync(int ingredientId)
        {
            try
            {
                // get ingredient from database
                IngredientModel ingredientToDelete = (await _repository.Ingredients.GetByExpressionAsync(x => x.Id == ingredientId)).SingleOrDefault();

                if (ingredientToDelete != null)
                {
                    // try to delete it
                    _repository.Ingredients.Delete(ingredientToDelete);

                    // save changes in database context
                    await _repository.SaveChangesAsync();

                    // return service result with Deleted status
                    return new ServiceResult(ResultType.Deleted);                
                }
                
                return new ServiceResult(ResultType.Error, new List<string> { $"Ingredient with id {ingredientId} was not found in database" });                
            }
            catch (Exception e)
            {
                // catch exception and pass errors to controller
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Updates price of ingredient stored in database with id value passed by parameter
        /// </summary>
        /// <param name="price">Defines which size of ingredient should be edited and stores new price value</param>
        /// <param name="ingredientId">identifier of ingredient that will be updated</param>
        /// <returns>Ingredient with updated price or errors (depending on result type)</returns>
        public async Task<IServiceResult<IngredientModel>> UpdatePriceAsync(IngredientDetailsToCreateDto price, int ingredientId)
        {
            try
            {
                // find ingredient with specific id value, include all prices of it 
                IngredientModel ingredientToUpdate = (await _repository.Ingredients.GetByExpressionAsync(x => x.Id == ingredientId, i => i.Include(id => id.IngredientDetails))).SingleOrDefault();

                // get price that will be updated
                IngredientDetailsModel priceToUpdate = ingredientToUpdate.IngredientDetails.SingleOrDefault(x => x.Size == price.Size);

                if (priceToUpdate != null)
                {
                    // update price
                    priceToUpdate.Price = price.Price;

                    // update database entry and save context changes
                    _repository.Ingredients.Update(ingredientToUpdate);
                    await _repository.SaveChangesAsync();

                    await UpdatePizzasPrices(ingredientId);

                    return new ServiceResult<IngredientModel>(ResultType.Edited, ingredientToUpdate);
                }

                return new ServiceResult<IngredientModel>(ResultType.Error, new List<string> { "Price has not been found" });
            }
            catch(Exception e)
            {
                // catch errors and pass them to controller
                return new ServiceResult<IngredientModel>(ResultType.Error, new List<string> { e.Message });
            }

        }

        #region PrivateMethods
        private async Task UpdatePizzasPrices(int ingredientId)
        {
            // update price of all pizzas that include edited ingredient
            List<PizzaModel> pizzasToUpdate = (await _repository.Pizzas.GetByExpressionAsync(x => x.Id > 0,
                i => i.Include(p => p.PizzaIngredients).ThenInclude(p => p.Ingredient).ThenInclude(p => p.IngredientDetails).Include(p => p.PizzaDetails).ThenInclude(s => s.Starter)))
                                                .Where(p => p.PizzaIngredients.Any(x => x.IngredientId == ingredientId)).ToList();
            if (pizzasToUpdate.Count > 0)
            {
                foreach (PizzaModel pizza in pizzasToUpdate)
                {
                    UpdateTotalPizzaPrices(pizza);
                    _repository.Pizzas.Update(pizza);
                }

                // save context changes
                await _repository.SaveChangesAsync();
            }
        }
        #endregion
    }
}
