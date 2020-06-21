using FoodOrderApp.Models.PizzaModels.DetailModels;
using FoodOrderApp.Models.PizzaModels.PhotoModels;
using System.Collections.Generic;

namespace FoodOrderApp.Models.PizzaModels
{
    public class PizzaModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<PizzaDetailsModel> PizzaDetails { get; set; }

        public ICollection<PizzaIngredientsModel> PizzaIngredients { get; set; }

        public PhotoModel Photo { get; set; }
    }
}
