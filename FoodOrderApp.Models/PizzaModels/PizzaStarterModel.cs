using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.PizzaModels
{
    public class PizzaStarterModel
    {
        public int PizzaId { get; set; }
        public int StarterId { get; set; }

        public PizzaModel Pizza { get; set; }
        public StarterModel Starter { get; set; }
    }
}
