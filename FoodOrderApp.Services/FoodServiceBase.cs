using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.DetailModels;
using System.Linq;

namespace FoodOrderApp.Services
{
    public abstract class FoodServiceBase
    {
        protected readonly IUnitOfWork _repository;

        public FoodServiceBase(IUnitOfWork repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Counts total prices of pizza
        /// </summary>
        /// <param name="pizza">data of pizza that total prices will be counted for</param>
        /// <returns>Total price of pizza (including starter and all ingredients)</returns>
        protected void UpdateTotalPizzaPrices(PizzaModel pizza)
        {
            decimal total = 0;
            foreach (PizzaDetailsModel pizzaDetails in pizza.PizzaDetails)
            {
                // get starter price
                total = pizzaDetails.Starter.Price;

                // add ingredients prices
                foreach (PizzaIngredientsModel pizzaIngredient in pizza.PizzaIngredients)
                {
                    // sum all prices
                    total += pizzaIngredient.Ingredient.IngredientDetails.SingleOrDefault(p => p.Size == pizzaDetails.Size).Price;
                }

                pizzaDetails.TotalPrice = total;
            }
        }
    }
}
