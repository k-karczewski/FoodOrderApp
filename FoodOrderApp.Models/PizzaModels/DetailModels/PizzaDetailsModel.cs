using FoodOrderApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.PizzaModels.DetailModels
{
    public class PizzaDetailsModel
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public SizeEnum Size { get; set; }


        public int PizzaId { get; set; }
        public PizzaModel Pizza { get; set; }


        public int StarterId { get; set; }
        public StarterModel Starter { get; set; }


        public ICollection<PizzaOrderModel> PizzaOrders { get; set; }
    }
}
