using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.PizzaModels.PriceModels
{
    public class PriceModel
    {
        public int Id { get; set; }
        public SizeEnum Size { get; set; }
        public decimal Price { get; set; }
    }

    public enum SizeEnum
    {
        Small,
        Medium,
        Big,
        Large
    }
}
