using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PriceModels;
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
        /// Creates new ingredient
        /// </summary>
        /// <param name="newObject">ingredient to be added to database</param>
        /// <returns>Ingredient that has been added to database or errors (depending on result type)</returns>
        public async Task<IServiceResult<IngredientModel>> CreateAsync(IngredientModel newObject)
        {
            try
            {
                // check if ingredient with that name already exists in db
                bool doesExistInDb = (await _repository.Ingredients.GetByExpressionAsync(x => x.Name == newObject.Name)).SingleOrDefault() == null ? false : true;

                if(doesExistInDb == false)
                {    
                    // try to add ingredient to database
                    bool result = await _repository.Ingredients.CreateAsync(newObject);

                    // if ingredient was added successfully
                    if(result == true)
                    {
                        // save changes in database context
                        await _repository.SaveChangesAsync();

                        // return Ok service result with created object
                        return new ServiceResult<IngredientModel>(ResultType.Created, newObject);
                    }
                
                    // result equals false, throw exception that ingredient was not added
                    throw new Exception("Cannot add new ingredient");
                }
                else
                {
                    throw new Exception($"Ingredient with name {newObject.Name} already exists in database");
                }
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
                    bool result = await _repository.Ingredients.DeleteAsync(ingredientToDelete);

                    // if deletion was successful
                    if (result == true)
                    {
                        // save changes in database context
                        await _repository.SaveChangesAsync();

                        // return service result with Deleted status
                        return new ServiceResult(ResultType.Deleted);
                    }

                    // if ingredient was not removed successfully throw exception with error message
                    throw new Exception($"Cannot delete ingredient with id {ingredientId}");
                }
                else
                {
                    throw new Exception($"Ingredient with id {ingredientId} was not found in database");
                }
            }
            catch (Exception e)
            {
                // catch exception and pass errors to controller
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Gets all ingredients from database with their prices
        /// </summary>
        /// <returns>List of all ingredients or errors (depending on result type)</returns>
        public async Task<IServiceResult<List<IngredientModel>>> GetAsync()
        {
            try
            {
                // try to get all ingredients
                List<IngredientModel> ingredients = (await _repository.Ingredients.GetByExpressionAsync(x => x.Id > 0, i => i.Include(p => p.Prices))).ToList();

                // if no exception occured during get operation return correct status 
                return new ServiceResult<List<IngredientModel>>(ResultType.Correct, ingredients);
            }
            catch(Exception e)
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
                IngredientModel result = (await _repository.Ingredients.GetByExpressionAsync(x => x.Id == id, i => i.Include(p => p.Prices))).SingleOrDefault();

                // if ingredient was found
                if (result != null)
                {
                    // return it to controller
                    return new ServiceResult<IngredientModel>(ResultType.Correct, result);
                }

                // throw exception otherwise
                throw new Exception($"Object with id: {id} has not been found");
            }
            catch (Exception e)
            {
                // catch errors and pass them to controller
                return new ServiceResult<IngredientModel>(ResultType.Error, new List<string>() { e.Message });
            }
        }

        /// <summary>
        /// Updates price of ingredient stored in database with id value passed by parameter
        /// </summary>
        /// <param name="price">Defines which size of ingredient should be edited and stores new price value</param>
        /// <param name="ingredientId">identifier of ingredient that will be updated</param>
        /// <returns>Ingredient with updated price or errors (depending on result type)</returns>
        public async Task<IServiceResult<IngredientModel>> UpdatePriceAsync(IngredientPriceModel price, int ingredientId)
        {
            try
            {
                // find ingredient with specific id value, include all prices of it 
                IngredientModel ingredientToUpdate = (await _repository.Ingredients.GetByExpressionAsync(x => x.Id == ingredientId, i => i.Include(p => p.Prices))).SingleOrDefault();

                // get price that will be updated
                IngredientPriceModel priceToUpdate = ingredientToUpdate.Prices.SingleOrDefault(x => x.Size == price.Size);

                if(priceToUpdate != null)
                {
                    // update price
                    priceToUpdate.Price = price.Price;

                    // update database entry and save context changes
                    await _repository.Ingredients.UpdateAsync(ingredientToUpdate);
                    await _repository.SaveChangesAsync();

                    // update price of all pizzas that include edited ingredient
                    List<PizzaModel> pizzasToUpdate = (await _repository.Pizzas.GetByExpressionAsync(x => x.Id > 0, 
                        i => i.Include(p => p.PizzaIngredients).ThenInclude(p => p.Ingredient).ThenInclude(p => p.Prices).Include(s => s.Starter)))
                                                        .Where(p => p.PizzaIngredients.Any(x => x.IngredientId == ingredientId)).ToList();

                    if(pizzasToUpdate.Count > 0)
                    {                
                        foreach(PizzaModel pizza in pizzasToUpdate)
                        {
                            pizza.TotalPrice = CountTotalPizzaPrice(pizza);
                            await _repository.Pizzas.UpdateAsync(pizza);
                        }

                        // save context changes
                        await _repository.SaveChangesAsync();
                    }

                    return new ServiceResult<IngredientModel>(ResultType.Edited, ingredientToUpdate);
                }
                else
                {
                    // throw exception if price was not found
                    throw new Exception("Price has not been found");
                }
            }
            catch(Exception e)
            {
                // catch errors and pass them to controller
                return new ServiceResult<IngredientModel>(ResultType.Error, new List<string> { e.Message });
            }

        }
    }
}
