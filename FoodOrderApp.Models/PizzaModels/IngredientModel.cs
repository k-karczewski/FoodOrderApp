using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.PizzaModels.PriceModels
{
    public class IngredientModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<IngredientPriceModel> Prices { get; set; }

        public ICollection<PizzaIngredientsModel> PizzaIngredients { get; set; }
    }
}
