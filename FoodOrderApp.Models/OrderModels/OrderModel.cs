using FoodOrderApp.Models.Enums;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.OrderModels
{
    public class OrderModel
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }

        public int UserId { get; set; }
        public UserModel User { get; set; }


        public ICollection<PizzaOrderModel> PizzaOrders { get; set; }

        public OrderStatus Status { get; set; }
    }
}
