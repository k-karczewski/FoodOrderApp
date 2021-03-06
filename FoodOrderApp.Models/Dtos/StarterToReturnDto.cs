﻿using FoodOrderApp.Models.Enums;

namespace FoodOrderApp.Models.Dtos
{
    public class StarterToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SizeEnum Size { get; set; }
        public decimal Price { get; set; }
    }
}
