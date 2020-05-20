using FoodOrderApp.Models.PizzaModels.PriceModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.Dtos
{
    public class PizzaToCreateDto
    {
        public string Name { get; set; }
        public List<int> IngredientIds { get; set; }
    }
}
