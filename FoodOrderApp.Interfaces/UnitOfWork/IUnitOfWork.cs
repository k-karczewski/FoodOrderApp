﻿using FoodOrderApp.Interfaces.Repositories;
using FoodOrderApp.Models.OrderModels;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PhotoModels;
using FoodOrderApp.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
         IFoodOrderRepository<IngredientModel> Ingredients { get; }
         IFoodOrderRepository<PizzaModel> Pizzas { get; }
         IFoodOrderRepository<StarterModel> Starters { get; }
         IFoodOrderRepository<UserModel> Users { get; }
         IFoodOrderRepository<PhotoModel> Photos { get; }
         IFoodOrderRepository<OrderModel> Orders { get; }

         Task SaveChangesAsync();
    }
}
