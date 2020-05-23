using FoodOrderApp.Interfaces.Repositories;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PriceModels;
using FoodOrderApp.Services.ServiceResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Counts total price of pizza
        /// </summary>
        /// <param name="pizza">data of pizza that total price will be counted for</param>
        /// <returns>Total price of pizza (including starter and all ingredients)</returns>
        protected ICollection<PizzaPriceModel> CountTotalPizzaPrice(PizzaModel pizza)
        {
            List<decimal> prices = new List<decimal>();
            decimal total = 0;
            foreach (PizzaStarterModel pizzaStarter in pizza.PizzaStarters)
            {
                // get starter price
                total = pizzaStarter.Starter.Price;

                // add ingredients prices
                foreach (PizzaIngredientsModel pizzaIngredient in pizza.PizzaIngredients)
                {
                    // sum all prices
                    total += pizzaIngredient.Ingredient.Prices.SingleOrDefault(p => p.Size == pizzaStarter.Starter.Size).Price;
                }

                prices.Add(total);
            }

            pizza.TotalPrices = new List<PizzaPriceModel>();

            for(int i = 0; i < prices.Count; i++)
            {
                pizza.TotalPrices.Add(new PizzaPriceModel
                {
                    Size = (SizeEnum)i,
                    Price = prices[i]
                });
            }

            // return total prices
            return pizza.TotalPrices;
        }
    }
}
