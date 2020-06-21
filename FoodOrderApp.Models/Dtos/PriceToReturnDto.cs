﻿using FoodOrderApp.Models.Enums;

namespace FoodOrderApp.Models.Dtos
{
    public class PriceToReturnDto
    {
        public SizeEnum Size { get; set; }
        public decimal Price { get; set; }
    }
}
