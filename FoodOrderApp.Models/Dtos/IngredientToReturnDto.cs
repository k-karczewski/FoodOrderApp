using FoodOrderApp.Models.PizzaModels.PriceModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.Dtos
{
    public class IngredientToReturnDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
