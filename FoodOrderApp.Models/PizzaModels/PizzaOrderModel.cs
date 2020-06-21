﻿using FoodOrderApp.Models.OrderModels;
using FoodOrderApp.Models.PizzaModels.DetailModels;

namespace FoodOrderApp.Models.PizzaModels
{
    public class PizzaOrderModel
    {
        public int PizzaId { get; set; }

        public int PizzaDetailId { get; set; }
        public PizzaDetailsModel PizzaDetail { get; set; }

        public int OrderId { get; set; }
        public OrderModel Order { get; set; }
    }
}
