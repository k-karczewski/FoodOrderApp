using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.PizzaModels
{
    public class PizzaModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal TotalPrice { get; set; }

        public int StarterId { get; set; }
        public StarterModel Starter { get; set; }

        public ICollection<PizzaIngredientsModel> PizzaIngredients { get; set; }
    }
}
