using FoodOrderApp.Data.DataContext;
using FoodOrderApp.Data.Repositories;
using FoodOrderApp.Interfaces.Repositories;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.PizzaModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FoodOrderContext _context;
        public IFoodOrderRepository<IngredientModel> Ingredients { get; }
        public IFoodOrderRepository<PizzaModel> Pizzas { get; }
        public IFoodOrderRepository<StarterModel> Starters { get; }

        public UnitOfWork(FoodOrderContext context)
        {
            _context = context;
            Ingredients = new FoodOrderRepository<IngredientModel>(context);
            Pizzas = new FoodOrderRepository<PizzaModel>(context);
            Starters = new FoodOrderRepository<StarterModel>(context);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
