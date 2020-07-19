using FoodOrderApp.Models.Enums;
using FoodOrderApp.Models.PizzaModels.DetailModels;
using System.Collections.Generic;

namespace FoodOrderApp.Models.PizzaModels
{
    public class StarterModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public SizeEnum Size { get; set; }

        public ICollection<PizzaDetailsModel> Pizzas { get; set; }
    }
}
