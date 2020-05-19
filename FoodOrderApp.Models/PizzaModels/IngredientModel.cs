using FoodOrderApp.Models.PizzaModels.PriceModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.PizzaModels
{
    public class IngredientModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<IngredientPriceModel> Prices { get; set; }

        public ICollection<PizzaIngredientsModel> PizzaIngredients { get; set; }
    }
}
