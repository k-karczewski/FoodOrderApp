using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.PizzaModels.PriceModels
{
    public class StarterPriceModel : PriceModel
    {
        public int StarterId { get; set; }
        public StarterModel Starter { get; set; }
    }
}
