using FoodOrderApp.Models.PizzaModels.PriceModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.PizzaModels
{
    public class PizzaModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<PizzaPriceModel> TotalPrices { get; set; }

        public ICollection<PizzaStarterModel> PizzaStarters { get; set; }
        public ICollection<PizzaIngredientsModel> PizzaIngredients { get; set; }
    }
}
