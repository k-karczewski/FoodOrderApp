using FoodOrderApp.Models.PizzaModels.PriceModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.PizzaModels
{
    public class StarterModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<StarterPriceModel> Prices { get; set; }

        public ICollection<PizzaModel> Pizzas { get; set; }
    }
}
