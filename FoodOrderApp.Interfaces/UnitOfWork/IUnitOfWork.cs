using FoodOrderApp.Interfaces.Repositories;
using FoodOrderApp.Models.PizzaModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable
    {
         IFoodOrderRepository<IngredientModel> Ingredients { get; }
         IFoodOrderRepository<PizzaModel> Pizzas { get; }
         IFoodOrderRepository<StarterModel> Starters { get; }

         Task SaveChangesAsync();
    }
}
