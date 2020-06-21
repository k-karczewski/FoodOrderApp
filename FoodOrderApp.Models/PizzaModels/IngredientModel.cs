using FoodOrderApp.Models.PizzaModels.DetailModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodOrderApp.Models.PizzaModels
{
    public class IngredientModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<IngredientDetailsModel> IngredientDetails { get; set; }

        public ICollection<PizzaIngredientsModel> PizzaIngredients { get; set; }
    }
}
