using FoodOrderApp.Models.Enums;

namespace FoodOrderApp.Models.Dtos
{
    public class PizzaToOrderDto
    {
        public int PizzaId { get; set; }
        public SizeEnum Size { get; set; }
    }
}
