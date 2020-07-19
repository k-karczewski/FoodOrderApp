using FoodOrderApp.Models.Enums;
using FoodOrderApp.Models.OrderModels;
using System.Collections.Generic;

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
    }
}
