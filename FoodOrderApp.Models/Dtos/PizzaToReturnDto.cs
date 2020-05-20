using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.Dtos
{
    public class PizzaToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal TotalPrice { get; set; }

        public StarterToReturnDto Starter { get; set; }
        public ICollection<IngredientToReturnDto> Ingredients { get; set; }
    }
}
