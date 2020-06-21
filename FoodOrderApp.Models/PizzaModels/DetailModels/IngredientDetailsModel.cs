using FoodOrderApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodOrderApp.Models.PizzaModels.DetailModels
{
    public class IngredientDetailsModel
    {
        public int Id { get; set; }
        [Required]
        public SizeEnum Size { get; set; }
        [Required]
        public decimal Price { get; set; }

        public int IngredientId { get; set; }
        public IngredientModel Ingredient { get; set; }
    }
}
