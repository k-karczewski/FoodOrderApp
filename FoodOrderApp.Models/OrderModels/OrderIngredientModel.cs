using FoodOrderApp.Models.PizzaModels;

namespace FoodOrderApp.Models.OrderModels
{
    public class OrderIngredientModel
    {
        public int OrderItemId { get; set; }
        public OrderItemModel OrderItem { get; set; }

        public int IngredientId { get; set; }
        public IngredientModel Ingredient { get; set; }
    }
}
