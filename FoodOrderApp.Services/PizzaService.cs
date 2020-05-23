using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PriceModels;
using FoodOrderApp.Services.ServiceResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FoodOrderApp.Services
{
    public class PizzaService : FoodServiceBase, IPizzaService
    {
        public PizzaService(IUnitOfWork repository) : base(repository) { }

        /// <summary>
        /// Gets all pizzas from database including ingredients, their prices and starter
        /// </summary>
        /// <returns>List of pizza data or list of errors</returns>
        public async Task<IServiceResult<List<PizzaToReturnDto>>> GetAsync()
        {
            try
            {
                List<PizzaToReturnDto> pizzasToReturn = new List<PizzaToReturnDto>();

                // get all pizzas with all child objects
                List<PizzaModel> pizzas =
                    (await _repository.Pizzas.GetByExpressionAsync(x => x.Id > 0, 
                                                   i => i.Include(s => s.PizzaStarters).ThenInclude(s => s.Starter).
                                                   Include(x => x.PizzaIngredients).ThenInclude(i => i.Ingredient).ThenInclude(p => p.Prices).
                                                   Include(tp => tp.TotalPrices))).ToList();

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

                // throw exception if no pizza object was found in database
                throw new Exception("Cannot load pizzas from database");
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
        public async Task<IServiceResult<PizzaModel>> GetByIdAsync(int id)
        {
            try
            {
                // try to get pizza with all child objects
                PizzaModel pizza =
                    (await _repository.Pizzas.GetByExpressionAsync(x => x.Id == id, 
                                                    i => i.Include(s => s.PizzaStarters).ThenInclude(s => s.Starter).
                                                    Include(x => x.PizzaIngredients).ThenInclude(i => i.Ingredient).ThenInclude(p => p.Prices).
                                                    Include(tp => tp.TotalPrices))).SingleOrDefault();

                if (pizza != null)
                {
                    // return the object if it was found in database
                    return new ServiceResult<PizzaModel>(ResultType.Correct, pizza);
                }

                // throw exception otherwise
                throw new Exception("Cannot load pizza from database");
            }
            catch (Exception e)
            {
                // catch exception and pass errors to controller
                return new ServiceResult<PizzaModel>(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Gets all pizzas with name passed by parameter
        /// </summary>
        /// <param name="name">name of pizza</param>
        /// <returns>List of pizzas with specific name</returns>
        public async Task<IServiceResult<List<PizzaToReturnDto>>> GetByName(string name)
        {
            try
            {
                // get pizzas that have name equal parameter value
                List<PizzaModel> pizzas = (await _repository.Pizzas.GetByExpressionAsync(x => x.Name.ToLower() == name.ToLower(),
                                            i => i.Include(s => s.PizzaStarters).ThenInclude(s => s.Starter).
                                                   Include(x => x.PizzaIngredients).ThenInclude(i => i.Ingredient).ThenInclude(p => p.Prices).
                                                   Include(tp => tp.TotalPrices))).ToList();

                if (pizzas != null && pizzas.Count > 0)
                {
                    List<PizzaToReturnDto> pizzasToReturn = new List<PizzaToReturnDto>();

                    // convert them to objects to retun
                    foreach (PizzaModel p in pizzas)
                    {
                        pizzasToReturn.Add(CreatePizzaToReturn(p));
                    }

                    // return converted pizza objects
                    return new ServiceResult<List<PizzaToReturnDto>>(ResultType.Correct, pizzasToReturn);
                }

                // throw exception if pizza with this name was not found in database
                throw new Exception($"Pizzas with name: {name} were not found");
            }
            catch (Exception e)
            {
                // catch exception and pass errors to controller
                return new ServiceResult<List<PizzaToReturnDto>>(ResultType.Error, new List<string> { e.Message });
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
                PizzaModel pizza =
                    (await _repository.Pizzas.GetByExpressionAsync(x => x.Id == pizzaId, i => i.Include(x => x.PizzaIngredients).
                                                   ThenInclude(z => z.Ingredient).ThenInclude(p => p.Prices).Include(s => s.PizzaStarters).ThenInclude(s => s.Starter))).SingleOrDefault();
                // if pizza was found
                if (pizza != null)
                {
                    // update price and save context changes
                    pizza.TotalPrices = CountTotalPizzaPrice(pizza);
                    await _repository.Pizzas.UpdateAsync(pizza);

                    return new ServiceResult<PizzaModel>(ResultType.Correct, pizza);
                }

                // throw exception that pizza with this id value was not found in database
                throw new Exception($"Pizza with id {pizzaId} was not found");
            }
            catch (Exception e)
            {
                // catch exception and pass errors to controller
                return new ServiceResult<PizzaModel>(ResultType.Error, new List<string> { e.Message });
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
                PizzaModel pizza =
                    (await _repository.Pizzas.GetByExpressionAsync(x => x.Id == pizzaId, i => i.Include(x => x.PizzaIngredients).ThenInclude(z => z.Ingredient).
                                       ThenInclude(p => p.Prices).Include(s => s.PizzaStarters).ThenInclude(s => s.Starter))).SingleOrDefault();

                if (pizza != null)
                {
                    // delete the pizza and save context changes
                    await _repository.Pizzas.DeleteAsync(pizza);
                    await _repository.SaveChangesAsync();

                    return new ServiceResult(ResultType.Deleted);
                }

                throw new Exception($"Cannot delete pizza with id {pizzaId}");
            }
            catch (Exception e)
            {
                // catch exception and pass errors to controller
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }
        }
        /// <summary>
        /// Creates new pizza in database
        /// </summary>
        /// <param name="pizzaToCreate">data of pizza to be created</param>
        /// <returns>Created pizza or errors that occured during creation (depending on returned ServiceResult state)</returns
        public async Task<IServiceResult<PizzaToReturnDto>> CreateAsync(PizzaToCreateDto pizzaToCreate)
        {
            try
            {
                PizzaModel nameUsed = (await _repository.Pizzas.GetByExpressionAsync(x => x.Name.ToLower() == pizzaToCreate.Name.ToLower())).FirstOrDefault();

                if (nameUsed == null)
                {
                    PizzaModel createdPizza = await CreatePizza(pizzaToCreate);
                    PizzaToReturnDto pizzaToReturn = CreatePizzaToReturn(createdPizza);

                    return new ServiceResult<PizzaToReturnDto>(ResultType.Created, pizzaToReturn);
                }
                else
                {
                    throw new Exception($"Pizza name: {pizzaToCreate.Name} is already taken");
                }
            }
            catch (Exception e)
            {
                return new ServiceResult<PizzaToReturnDto>(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Adds ingredient to existing pizza
        /// </summary>
        /// <param name="pizzaName">name of pizza that ingredient will be added for</param>
        /// <param name="ingredientId">identifier of ingredient</param>
        /// <returns>PizzaToReturn object or list of errors</returns>
        public async Task<IServiceResult<List<PizzaToReturnDto>>> AddIngredientAsync(string pizzaName, int ingredientId)
        {
            try
            {
                List<PizzaModel> pizzasToUpdate = (await _repository.Pizzas.GetByExpressionAsync(x => x.Name.ToLower() == pizzaName.ToLower(),
                                            i => i.Include(pi => pi.PizzaIngredients).ThenInclude(i => i.Ingredient).ThenInclude(p => p.Prices).Include(s => s.PizzaStarters).ThenInclude(s => s.Starter))).ToList();

                // if pizza was found
                if (pizzasToUpdate != null && pizzasToUpdate.Count > 0)
                {
                    // check if pizza already have the ingredient to be added
                    bool isIncluded = pizzasToUpdate[0].PizzaIngredients.SingleOrDefault(x => x.IngredientId == ingredientId) == null ? false : true;

                    if (isIncluded == false)
                    {
                        // take the ingredient from database
                        IngredientModel ingredientToAdd = (await _repository.Ingredients.GetByExpressionAsync(x => x.Id == ingredientId, i => i.Include(p => p.Prices))).SingleOrDefault();

                        // update all sizes of pizzas
                        foreach(PizzaModel pizzaToUpdate in pizzasToUpdate)
                        {
                            pizzaToUpdate.PizzaIngredients.Add(
                                new PizzaIngredientsModel
                                {
                                    Ingredient = ingredientToAdd,
                                    Pizza = pizzaToUpdate
                                }
                            );

                            pizzaToUpdate.TotalPrices = CountTotalPizzaPrice(pizzaToUpdate);

                            bool updateResult = await _repository.Pizzas.UpdateAsync(pizzaToUpdate);

                            if(updateResult == false)
                            {
                                throw new Exception("Error during edition of pizza object");
                            }
                        }

                        await _repository.SaveChangesAsync();

                        List<PizzaToReturnDto> pizzasToReturn = new List<PizzaToReturnDto>();

                        foreach(PizzaModel pizza in pizzasToUpdate)
                        {
                            pizzasToReturn.Add(CreatePizzaToReturn(pizza));
                        }

                        return new ServiceResult<List<PizzaToReturnDto>>(ResultType.Edited, pizzasToReturn);
                    }
                    else
                    {
                        throw new Exception($"Ingredient with id {ingredientId} is already included in pizza with name {pizzaName}");
                    }
                }
                else
                {
                    throw new Exception($"Pizza with name {pizzaName} was not found in database");
                }
            }
            catch (Exception e)
            {
                return new ServiceResult<List<PizzaToReturnDto>>(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Deletes ingredient from existing pizza
        /// </summary>
        /// <param name="pizzaName">name of pizza that ingredient will be removed from</param>
        /// <param name="ingredientId">identifier of ingredient</param>
        /// <returns>PizzaToReturn object or list of errors</returns>
        public async Task<IServiceResult<List<PizzaToReturnDto>>> DeleteIngredientAsync(string pizzaName, int ingredientId)
        {
            try
            {
                List<PizzaModel> pizzasToUpdate = (await _repository.Pizzas.GetByExpressionAsync(x => x.Name == pizzaName, 
                                                i => i.Include(pi => pi.PizzaIngredients).ThenInclude(i => i.Ingredient).ThenInclude(p => p.Prices).Include(s => s.PizzaStarters).ThenInclude(s => s.Starter))).ToList();

                if(pizzasToUpdate != null && pizzasToUpdate.Count > 0)
                {
                    bool isIncluded = pizzasToUpdate[0].PizzaIngredients.SingleOrDefault(x => x.IngredientId == ingredientId) == null ? false : true;
                    
                    if(isIncluded == true)
                    {
                        List<PizzaToReturnDto> pizzasToReturn = new List<PizzaToReturnDto>();

                        foreach(PizzaModel pizzaToUpdate in pizzasToUpdate)
                        {
                            pizzaToUpdate.PizzaIngredients.Remove(pizzaToUpdate.PizzaIngredients.SingleOrDefault(x => x.IngredientId == ingredientId));
                            pizzaToUpdate.TotalPrices = CountTotalPizzaPrice(pizzaToUpdate);

                            bool result = await _repository.Pizzas.UpdateAsync(pizzaToUpdate);

                            if(!result)
                            {
                                throw new Exception($"Error during editing ingredients of pizza {pizzaName}");
                            }

                            pizzasToReturn.Add(CreatePizzaToReturn(pizzaToUpdate));
                        }

                        await _repository.SaveChangesAsync();

                        return new ServiceResult<List<PizzaToReturnDto>>(ResultType.Edited, pizzasToReturn);
                    }
                    else
                    {
                        throw new Exception($"Cannot delete ingredient with id {ingredientId} from pizza {pizzaName} because it is not included in pizza {pizzaName}");
                    }
                }
                else
                {
                    throw new Exception($"Pizza {pizzaName} was not found in database");
                }
            }
            catch(Exception e)
            {
                return new ServiceResult<List<PizzaToReturnDto>>(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Creates PizzaModel object for all starters that are available in database.
        /// </summary>
        /// <param name="pizzaToCreate">Data of new pizza</param>
        /// <returns>List of new created pizzas</returns>
        private async Task<PizzaModel> CreatePizza(PizzaToCreateDto pizzaToCreate)
        {
            List<PizzaModel> createdPizzas = new List<PizzaModel>();
            List<StarterModel> starters = (await _repository.Starters.GetByExpressionAsync(x => x.Id > 0)).ToList();
            List<IngredientModel> ingredients = new List<IngredientModel>();

            // get ingredients with their prices
            foreach (int ingredientId in pizzaToCreate.IngredientIds)
            {
                ingredients.Add((await _repository.Ingredients.GetByExpressionAsync(x => x.Id == ingredientId, i => i.Include(p => p.Prices))).SingleOrDefault());
            }

            PizzaModel newPizza = new PizzaModel
            {
                Name = pizzaToCreate.Name,
                PizzaStarters = CreatePizzaStarters(starters),
                PizzaIngredients = new List<PizzaIngredientsModel>()
            };

            foreach (IngredientModel ingredient in ingredients)
            {
                newPizza.PizzaIngredients.Add(new PizzaIngredientsModel
                {
                    Ingredient = ingredient
                });
            }

            newPizza.TotalPrices = CountTotalPizzaPrice(newPizza);

            bool result = await _repository.Pizzas.CreateAsync(newPizza);

            if (result == false)
            {
                // throw exception if addition of new pizza failed
                throw new Exception("Error during addition of pizza");
            }

            // save changes if operation succeded
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
            List<IngredientToReturnDto> ingredientsToReturn = new List<IngredientToReturnDto>();

            // convert included ingredients
            foreach (PizzaIngredientsModel i in pizza.PizzaIngredients)
            {
                IngredientToReturnDto ingredientToReturn = new IngredientToReturnDto
                {
                    Name = i.Ingredient.Name,
                    Prices = new List<PriceToReturnDto>()
                };

                foreach(IngredientPriceModel p in i.Ingredient.Prices)
                {
                    ingredientToReturn.Prices.Add(new PriceToReturnDto
                    { 
                        Size = p.Size,
                        Price = p.Price
                    });
                }

                ingredientsToReturn.Add(ingredientToReturn);
            }


            List<StarterToReturnDto> startersToReturn = new List<StarterToReturnDto>();

            // create object to return
            PizzaToReturnDto pizzaToReturn = new PizzaToReturnDto
            {
                Id = pizza.Id,
                Name = pizza.Name,
                TotalPrices = new List<PriceToReturnDto>(),
                Ingredients = ingredientsToReturn
            };

            foreach (PizzaPriceModel p in pizza.TotalPrices)
            {
                pizzaToReturn.TotalPrices.Add(new PriceToReturnDto
                {
                    Size = p.Size,
                    Price = p.Price
                });
            }

            return pizzaToReturn;
        }

        private ICollection<PizzaStarterModel> CreatePizzaStarters(ICollection<StarterModel> starters)
        {
            List<PizzaStarterModel> pizzaStarters = new List<PizzaStarterModel>();

            foreach (StarterModel starter in starters)
            {
                PizzaStarterModel tmp = new PizzaStarterModel
                {
                    Starter = starter,
                };

                pizzaStarters.Add(tmp);
            }

            return pizzaStarters;
        }
    }
}
