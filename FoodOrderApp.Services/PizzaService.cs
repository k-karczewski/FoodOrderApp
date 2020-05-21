using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Services.ServiceResults;
using Microsoft.EntityFrameworkCore;
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
                    (await _repository.Pizzas.GetByExpressionAsync(x => x.Id > 0, i => i.Include(x => x.PizzaIngredients).ThenInclude(z => z.Ingredient).
                                                      ThenInclude(p => p.Prices).Include(s => s.Starter))).ToList();

                // if pizzas were found
                if(pizzas != null && pizzas.Count > 0)
                {
                    
                    foreach(PizzaModel p in pizzas)
                    {
                        // create dto objects for all pizzas taken from database
                        pizzasToReturn.Add(CreatePizzaToReturn(p));
                    }

                    return new ServiceResult<List<PizzaToReturnDto>>(ResultType.Correct, pizzasToReturn);
                }

                // throw exception if no pizza object was found in database
                throw new Exception("Cannot load pizzas from database");
            }
            catch(Exception e)
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
                    (await _repository.Pizzas.GetByExpressionAsync(x => x.Id == id, i => i.Include(x => x.PizzaIngredients).ThenInclude(z => z.Ingredient).
                                                      ThenInclude(p => p.Prices). Include(s => s.Starter))).SingleOrDefault();

                if (pizza != null)
                {
                    // return the object if it was found in database
                    return new ServiceResult<PizzaModel>(ResultType.Correct, pizza);
                }

                // throw exception otherwise
                throw new Exception("Cannot load pizza from database");
            }
            catch(Exception e)
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
                                            i => i.Include(s => s.Starter).Include(x => x.PizzaIngredients).ThenInclude(i => i.Ingredient).ThenInclude(p => p.Prices))).ToList();

                if(pizzas != null && pizzas.Count > 0)
                {
                    List<PizzaToReturnDto> pizzasToReturn = new List<PizzaToReturnDto>();

                    // convert them to objects to retun
                    foreach(PizzaModel p in pizzas)
                    {
                        pizzasToReturn.Add(CreatePizzaToReturn(p));
                    }

                    // return converted pizza objects
                    return new ServiceResult<List<PizzaToReturnDto>>(ResultType.Correct, pizzasToReturn);
                }

                // throw exception if pizza with this name was not found in database
                throw new Exception($"Pizzas with name: {name} were not found");
            }
            catch(Exception e)
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
                                                   ThenInclude(z => z.Ingredient).ThenInclude(p => p.Prices).Include(s => s.Starter))).SingleOrDefault();         
                // if pizza was found
                if (pizza != null)
                {
                    // update price and save context changes
                    pizza.TotalPrice = CountTotalPizzaPrice(pizza);
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
                                       ThenInclude(p => p.Prices).Include(s => s.Starter))).SingleOrDefault();

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

        public async Task<IServiceResult<List<PizzaModel>>> CreateAsync(PizzaToCreateDto pizzaToCreate)
        {
            try
            {
                PizzaModel nameUsed = (await _repository.Pizzas.GetByExpressionAsync(x => x.Name.ToLower() == pizzaToCreate.Name.ToLower())).FirstOrDefault();

                if(nameUsed == null)
                {
                    List<PizzaModel> createdPizzas = await CreatePizza(pizzaToCreate);

                    return new ServiceResult<List<PizzaModel>>(ResultType.Created, createdPizzas);
                }
                else
                {
                    throw new Exception($"Pizza name: {pizzaToCreate.Name} is already taken");
                }
            }
            catch(Exception e)
            {
                return new ServiceResult<List<PizzaModel>> (ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Creates PizzaModel object for all starters that are available in database.
        /// </summary>
        /// <param name="pizzaToCreate">Data of new pizza</param>
        /// <returns>List of new created pizzas</returns>
        private async Task<List<PizzaModel>> CreatePizza(PizzaToCreateDto pizzaToCreate)
        {
            List<PizzaModel> createdPizzas = new List<PizzaModel>();
            List<StarterModel> starters = (await _repository.Starters.GetByExpressionAsync(x => x.Id > 0)).ToList();
            List<IngredientModel> ingredients = new List<IngredientModel>();

            // get ingredients with their prices
            foreach (int ingredientId in pizzaToCreate.IngredientIds)
            {
                ingredients.Add((await _repository.Ingredients.GetByExpressionAsync(x => x.Id == ingredientId, i => i.Include(p => p.Prices))).SingleOrDefault());
            }

            // create pizza for each starter
            for (int i = 0; i < starters.Count; i++)
            {
                PizzaModel newPizza = new PizzaModel
                {
                    Name = pizzaToCreate.Name,
                    Starter = starters[i],
                    PizzaIngredients = new List<PizzaIngredientsModel>()
                };

                foreach (IngredientModel ingredient in ingredients)
                {
                    newPizza.PizzaIngredients.Add(new PizzaIngredientsModel
                    {
                        Ingredient = ingredient
                    });
                }

                // count total price of new pizza
                newPizza.TotalPrice = CountTotalPizzaPrice(newPizza);

                bool result = await _repository.Pizzas.CreateAsync(newPizza);

                if (result == false)
                {
                    // throw exception if addition of new pizza failed
                    throw new Exception("Error during addition of pizzas");
                }

                createdPizzas.Add(newPizza);
            }

            // save changes if operation succeded
            await _repository.SaveChangesAsync();

            return createdPizzas;
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
                    Price = i.Ingredient.Prices.SingleOrDefault(x => x.Size == pizza.Starter.Size).Price
                };

                ingredientsToReturn.Add(ingredientToReturn);
            }

            // convert starter object
            StarterToReturnDto starterToReturn = new StarterToReturnDto
            {
                Id = pizza.Starter.Id,
                Name = pizza.Starter.Name,
                Size = pizza.Starter.Size,
                Price = pizza.Starter.Price
            };

            // create object to return
            PizzaToReturnDto pizzaToReturn = new PizzaToReturnDto
            {
                Id = pizza.Id,
                Name = pizza.Name,
                TotalPrice = pizza.TotalPrice,
                Starter = starterToReturn,
                Ingredients = ingredientsToReturn
            };

            return pizzaToReturn;
        }
    }
}
