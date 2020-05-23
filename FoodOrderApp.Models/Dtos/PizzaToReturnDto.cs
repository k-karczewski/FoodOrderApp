using FoodOrderApp.Models.PizzaModels.PriceModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.Dtos
{
    public class PizzaToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<PriceToReturnDto> TotalPrices { get; set; }
        //public ICollection<StarterToReturnDto> Starters { get; set; }
        public ICollection<IngredientToReturnDto> Ingredients { get; set; }
    }
}
