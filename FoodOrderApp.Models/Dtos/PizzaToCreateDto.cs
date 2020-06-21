using System.Collections.Generic;

namespace FoodOrderApp.Models.Dtos
{
    public class PizzaToCreateDto
    {
        public string Name { get; set; }
        public List<int> IngredientIds { get; set; }
    }
}
