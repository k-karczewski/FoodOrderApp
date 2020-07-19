using FoodOrderApp.Models.Enums;
using System.Collections.Generic;

namespace FoodOrderApp.Models.OrderModels
{
    public class OrderItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SizeEnum Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public ICollection<OrderIngredientModel> OrderItemIngredients { get; set; }

        public int OrderId { get; set; }
        public OrderModel Order { get; set; }
    }
}
