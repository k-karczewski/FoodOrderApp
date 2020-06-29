using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.Enums;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.DetailModels;
using FoodOrderApp.Services.ServiceResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderApp.Services
{
    public class PizzaService : FoodServiceBase, IPizzaService
    {
        public PizzaService(IUnitOfWork repository) : base(repository) { }

        /// <summary>
        /// Gets all pizzas from database including ingredients, their prices and starters
        /// </summary>
        /// <returns>List of pizza to return data or list of errors</returns>
        public async Task<IServiceResult<List<PizzaToReturnDto>>> GetAsync()
        {
            try
            {
                List<PizzaToReturnDto> pizzasToReturn = new List<PizzaToReturnDto>();

                // get all pizzas with all child objects
                List<PizzaModel> pizzas =
                    (await _repository.Pizzas.GetByExpressionAsync(x => x.Name != "",
                                                   i => i.Include(pd => pd.PizzaDetails).ThenInclude(s => s.Starter).
                                                   Include(x => x.PizzaIngredients).ThenInclude(i => i.Ingredient).ThenInclude(id => id.IngredientDetails).
                                                   Include(p => p.Photo))).ToList();

                // if pizzas were found
                if (pizzas != null && pizzas.Count > 0)
                {
                    foreach (PizzaModel p in pizzas)
                    {
                        // create dto objects for all pizzas taken from database
                        pizzasToReturn.Add(CreatePizzaToReturn(p));
                    }

                    return new ServiceResult<List<PizzaToReturnDto>>(ResultType.Correct, pizzasToReturn);
                }

                // return error if no pizza object was found in database
                return new ServiceResult<List<PizzaToReturnDto>>(ResultType.Error, new List<string> { "Cannot load pizzas from database" });
            }
            catch (Exception e)
            {
                // catch exception and pass errors to controller
                return new ServiceResult<List<PizzaToReturnDto>>(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Gets PizzaModel object by identifier
        /// </summary>
        /// <param name="id">id of pizza</param>
        /// <returns>Single PizzaModel object or list of errors</returns>
        public async Task<IServiceResult<PizzaToReturnDto>> GetByIdAsync(int id)
        {
            try
            {
                // try to get pizza with all child objects
                PizzaModel pizza = await GetPizzaById(id);

                if (pizza != null)
                {
                    PizzaToReturnDto pizzaToReturn = CreatePizzaToReturn(pizza);

                    // return the object if it was found in database
                    return new ServiceResult<PizzaToReturnDto>(ResultType.Correct, pizzaToReturn);
                }

                // pass error state to controller
                return new ServiceResult<PizzaToReturnDto>(ResultType.Error, new List<string> { "Cannot load pizza from database" });
            }
            catch (Exception e)
            {
                // catch exception and pass errors to controller
                return new ServiceResult<PizzaToReturnDto>(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Pizza stored with name passed by parameter
        /// </summary>
        /// <param name="name">name of pizza</param>
        /// <returns>Pizza to return object with specific name</returns>
        public async Task<IServiceResult<PizzaToReturnDto>> GetByNameAsync(string name)
        {
            try
            {
                // get pizza that have name equal parameter value
                PizzaModel pizza = await GetPizzasByName(name);

                if (pizza != null)
                {
                    // convert it to object to retun
                    PizzaToReturnDto pizzaToReturn = CreatePizzaToReturn(pizza);

                    // return converted pizza objects
                    return new ServiceResult<PizzaToReturnDto>(ResultType.Correct, pizzaToReturn);
                }

                // pass error if pizzas with this name was not found in database
                return new ServiceResult<PizzaToReturnDto>(ResultType.Error, new List<string> { $"Pizzas with name: {name} were not found" });
            }
            catch (Exception e)
            {
                // catch exception and pass errors to controller
                return new ServiceResult<PizzaToReturnDto>(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Creates new pizza in database
        /// </summary>
        /// <param name="pizzaToCreate">data of pizza to be created</param>
        /// <returns>Pizza data to return or errors that occured during creation (depending on returned ServiceResult state)</returns
        public async Task<IServiceResult<PizzaToReturnDto>> CreateAsync(PizzaToCreateDto pizzaToCreate)
        {
            try
            {
                PizzaModel nameUsed = await GetPizzasByName(pizzaToCreate.Name);

                if (nameUsed == null)
                {
                    PizzaModel createdPizza = await CreatePizza(pizzaToCreate);
                    PizzaToReturnDto pizzaToReturn = CreatePizzaToReturn(createdPizza);

                    return new ServiceResult<PizzaToReturnDto>(ResultType.Created, pizzaToReturn);
                }

                return new ServiceResult<PizzaToReturnDto>(ResultType.Error, new List<string> { $"Pizza name: {pizzaToCreate.Name} is already taken" });

            }
            catch (Exception e)
            {
                return new ServiceResult<PizzaToReturnDto>(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Deletes pizza with specific id from database
        /// </summary>
        /// <param name="pizzaId">identifier of pizza</param>
        /// <returns>Result of status Deleted or Error</returns>
        public async Task<IServiceResult> DeleteAsync(int pizzaId)
        {
            try
            {
                // get pizza to delete
                PizzaModel pizza = await GetPizzaById(pizzaId);

                if (pizza != null)
                {
                    // delete the pizza and save context changes
                    _repository.Pizzas.Delete(pizza);
                    await _repository.SaveChangesAsync();

                    return new ServiceResult(ResultType.Deleted);
                }

                return new ServiceResult(ResultType.Error, new List<string> { $"Cannot delete pizza with id {pizzaId}" });
            }
            catch (Exception e)
            {
                // catch exception and pass errors to controller
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Updates total price of pizza. The total price is based on stater price and included ingredients prices.
        /// </summary>
        /// <param name="pizzaId">id of pizza </param>
        /// <returns>Updated pizza model</returns>
        public async Task<IServiceResult<PizzaModel>> UpdatePriceAsync(int pizzaId)
        {
            try
            {
                // get pizza with id value of pizzaId
                PizzaModel pizza = await GetPizzaById(pizzaId);
                // if pizza was found
                if (pizza != null)
                {
                    // update price and save context changes
                    UpdateTotalPizzaPrices(pizza);
                    _repository.Pizzas.Update(pizza);

                    return new ServiceResult<PizzaModel>(ResultType.Correct, pizza);
                }

                return new ServiceResult<PizzaModel>(ResultType.Error, new List<string> { $"Pizza with id {pizzaId} was not found" });
            }
            catch (Exception e)
            {
                // catch exception and pass errors to controller
                return new ServiceResult<PizzaModel>(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Adds ingredient to existing pizza
        /// </summary>
        /// <param name="pizzaName">name of pizza that ingredient will be added for</param>
        /// <param name="ingredientId">identifier of ingredient</param>
        /// <returns>PizzaToReturn object or list of errors</returns>
        public async Task<IServiceResult<PizzaToReturnDto>> AddIngredientAsync(string pizzaName, int ingredientId)
        {
            try
            {
                PizzaModel pizzaToUpdate = await GetPizzasByName(pizzaName);

                // if pizza was found
                if (pizzaToUpdate != null)
                {
                    // check if pizza already have the ingredient to be added
                    bool isIncluded = pizzaToUpdate.PizzaIngredients.SingleOrDefault(x => x.IngredientId == ingredientId) == null ? false : true;

                    if (isIncluded == false)
                    {
                        // take the ingredient from database
                        IngredientModel ingredientToAdd = (await _repository.Ingredients.GetByExpressionAsync(x => x.Id == ingredientId, i => i.Include(p => p.IngredientDetails))).SingleOrDefault();

                        // update all sizes of pizza
                        pizzaToUpdate.PizzaIngredients.Add(new PizzaIngredientsModel { Ingredient = ingredientToAdd, Pizza = pizzaToUpdate });
                        UpdateTotalPizzaPrices(pizzaToUpdate);
                        _repository.Pizzas.Update(pizzaToUpdate);
                        await _repository.SaveChangesAsync();

                        // return new version of pizza object
                        PizzaToReturnDto pizzaToReturn = CreatePizzaToReturn(pizzaToUpdate);

                        return new ServiceResult<PizzaToReturnDto>(ResultType.Edited, pizzaToReturn);
                    }
                }

                return new ServiceResult<PizzaToReturnDto>(ResultType.Error, new List<string> { $"Error during addition of new ingredient to {pizzaName} pizza" });
            }
            catch (Exception e)
            {
                return new ServiceResult<PizzaToReturnDto>(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Deletes ingredient from existing pizza
        /// </summary>
        /// <param name="pizzaName">name of pizza that ingredient will be removed from</param>
        /// <param name="ingredientId">identifier of ingredient</param>
        /// <returns>PizzaToReturn object or list of errors</returns>
        public async Task<IServiceResult<PizzaToReturnDto>> DeleteIngredientAsync(string pizzaName, int ingredientId)
        {
            try
            {
                // get pizza to update
                PizzaModel pizzaToUpdate = await GetPizzasByName(pizzaName);

                if (pizzaToUpdate != null)
                {
                    bool isIncluded = pizzaToUpdate.PizzaIngredients.SingleOrDefault(x => x.IngredientId == ingredientId) == null ? false : true;

                    // if pizza contains the ingredient to delete
                    if (isIncluded == true)
                    {
                        pizzaToUpdate.PizzaIngredients.Remove(pizzaToUpdate.PizzaIngredients.SingleOrDefault(x => x.IngredientId == ingredientId));
                        UpdateTotalPizzaPrices(pizzaToUpdate);

                        //update pizza price after deletion of ingredient
                        _repository.Pizzas.Update(pizzaToUpdate);
                        await _repository.SaveChangesAsync();

                        PizzaToReturnDto pizzaToReturn = CreatePizzaToReturn(pizzaToUpdate);

                        // return edited version of pizza object
                        return new ServiceResult<PizzaToReturnDto>(ResultType.Edited, pizzaToReturn);
                    }
                }
                
                // ingredient has not been deleted - pass error state to controller
                return new ServiceResult<PizzaToReturnDto>(ResultType.Error, new List<string> { $"Error during deletion of new ingredient to {pizzaName} pizza" });     
            }
            catch (Exception e)
            {
                return new ServiceResult<PizzaToReturnDto>(ResultType.Error, new List<string> { e.Message });
            }
        }

        #region PrivateMethods
        /// <summary>
        /// Creates PizzaModel object for all starters that are available in database.
        /// </summary>
        /// <param name="pizzaToCreate">Data of new pizza</param>
        /// <returns>List of new created pizzas</returns>
        private async Task<PizzaModel> CreatePizza(PizzaToCreateDto pizzaToCreate)
        {
            List<IngredientModel> ingredients = new List<IngredientModel>();
            List<StarterModel> starters = (await _repository.Starters.GetByExpressionAsync(x => x.Id > 0)).ToList();

            // get ingredients with their prices
            foreach (int ingredientId in pizzaToCreate.IngredientIds)
            {
                ingredients.Add((await _repository.Ingredients.GetByExpressionAsync(x => x.Id == ingredientId, i => i.Include(p => p.IngredientDetails))).SingleOrDefault());
            }

            PizzaModel newPizza = new PizzaModel
            {
                Name = pizzaToCreate.Name,
                PizzaDetails = CreatePizzaDetails(starters),
                PizzaIngredients = new List<PizzaIngredientsModel>(),
                Category = pizzaToCreate.Category
            };

            foreach (IngredientModel ingredient in ingredients)
            {
                newPizza.PizzaIngredients.Add(new PizzaIngredientsModel
                {
                    Ingredient = ingredient,
                    Pizza = newPizza
                });
            }

            UpdateTotalPizzaPrices(newPizza);

            await _repository.Pizzas.CreateAsync(newPizza);

            // save changes
            await _repository.SaveChangesAsync();

            return newPizza;
        }

        /// <summary>
        /// Converts PizzaModel object to PizzaToReturnDto
        /// </summary>
        /// <param name="pizza">PizzaModel object to be converted</param>
        /// <returns>Converted pizza object</returns>
        private PizzaToReturnDto CreatePizzaToReturn(PizzaModel pizza)
        {
            List<string> ingredientsToReturn = new List<string>
            {
                "sos",
                "ser",
                "oregano"
            };
            // convert included ingredients
            foreach (PizzaIngredientsModel i in pizza.PizzaIngredients)
            {
                ingredientsToReturn.Add(i.Ingredient.Name);
            }

            // create object to return
            PizzaToReturnDto pizzaToReturn = new PizzaToReturnDto
            {
                Id = pizza.Id,
                Name = pizza.Name,
                TotalPrices = new List<PriceToReturnDto>(),
                Ingredients = ingredientsToReturn,
                Category = Enum.GetName(typeof(PizzaCategory), pizza.Category)
            };

            if(pizza.Photo != null)
            {
                pizzaToReturn.PhotoUrl = pizza.Photo.Url;
            }

            foreach (PizzaDetailsModel p in pizza.PizzaDetails)
            {
                pizzaToReturn.TotalPrices.Add(new PriceToReturnDto
                {
                    Size = p.Size,
                    Price = p.TotalPrice
                });
            }

            return pizzaToReturn;
        }

        /// <summary>
        /// Convers StarterModel objects to PizzaStarterModel objects
        /// </summary>
        /// <param name="starters">collection of StarterModel objects</param>
        /// <returns>Collection of PizzaStarterModel objects</returns>
        private ICollection<PizzaDetailsModel> CreatePizzaDetails(ICollection<StarterModel> starters)
        {

            List<PizzaDetailsModel> pizzaDetails = new List<PizzaDetailsModel>();

            foreach (StarterModel starter in starters)
            {
                PizzaDetailsModel tmp = new PizzaDetailsModel
                {
                    Size = (SizeEnum)starters.IndexOf(starter),
                    StarterId = starter.Id,
                    Starter = starter,
                };

                pizzaDetails.Add(tmp);
            }

            return pizzaDetails;
        }

        /// <summary>
        /// Gets from repository all pizzas with name passed by parameter
        /// </summary>
        /// <param name="name">value of name</param>
        /// <returns>List of pizzas that matches the name</returns>
        private async Task<PizzaModel> GetPizzasByName(string name)
        {
            return (await _repository.Pizzas.GetByExpressionAsync(x => x.Name.ToLower() == name.ToLower(),
                                                i => i.Include(pi => pi.PizzaIngredients).ThenInclude(i => i.Ingredient).ThenInclude(p => p.IngredientDetails).
                                                Include(s => s.PizzaDetails).ThenInclude(s => s.Starter).
                                                Include(p => p.Photo))).FirstOrDefault();
        }

        /// <summary>
        /// Gets pizza from repository with id passed by parameter
        /// </summary>
        /// <param name="id">value of id</param>
        /// <returns>Pizza model that matches the id</returns>
        private async Task<PizzaModel> GetPizzaById(int id)
        {
            return (await _repository.Pizzas.GetByExpressionAsync(x => x.Id == id,
                                                    i => i.Include(s => s.PizzaDetails).ThenInclude(s => s.Starter).
                                                    Include(x => x.PizzaIngredients).ThenInclude(i => i.Ingredient).ThenInclude(p => p.IngredientDetails).
                                                    Include(p => p.Photo))).SingleOrDefault();
        }

        #endregion
    }
}
