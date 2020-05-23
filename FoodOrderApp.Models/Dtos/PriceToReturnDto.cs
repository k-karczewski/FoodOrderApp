using FoodOrderApp.Models.PizzaModels.PriceModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.Dtos
{
    public class PriceToReturnDto
    {
        public SizeEnum Size { get; set; }
        public decimal Price { get; set; }
    }
}
