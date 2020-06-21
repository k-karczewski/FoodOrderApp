using System.Collections.Generic;

namespace FoodOrderApp.Models.Dtos
{
    public class PizzaToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }

        public ICollection<PriceToReturnDto> TotalPrices { get; set; }
        public ICollection<IngredientToReturnDto> Ingredients { get; set; }
    }
}
