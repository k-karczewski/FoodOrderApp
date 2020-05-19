using FoodOrderApp.Models.PizzaModels.PriceModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.PizzaModels.PriceModels
{
    public class IngredientPriceModel : PriceModel
    {
        public int IngredientId { get; set; }
        public IngredientModel Ingredient { get; set; }
    }
}
