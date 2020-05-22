using FoodOrderApp.Interfaces.Repositories;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.PizzaModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Tests._Fakes.Data
{
    public class UnitOfWork_Fake : IUnitOfWork
    {
        public IFoodOrderRepository<IngredientModel> Ingredients { get; }
        public IFoodOrderRepository<PizzaModel> Pizzas { get; }
        public IFoodOrderRepository<StarterModel> Starters { get; }

        public UnitOfWork_Fake()
        {
            Ingredients = new FoodOrderRepository_Fake<IngredientModel>();
            Pizzas = new FoodOrderRepository_Fake<PizzaModel>();
            Starters = new FoodOrderRepository_Fake<StarterModel>();
        }

        public async ValueTask DisposeAsync()
        {
            return;
        }

        public async Task SaveChangesAsync()
        {
            return;
        }
    }
}
