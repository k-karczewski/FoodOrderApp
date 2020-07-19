using FoodOrderApp.Models.Enums;
using System.Collections.Generic;

namespace FoodOrderApp.Models.Dtos
{
    public class PizzaToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string Category { get; set; }

        public ICollection<PriceToReturnDto> TotalPrices { get; set; }
        public ICollection<int> Ingredients { get; set; }
    }
}
