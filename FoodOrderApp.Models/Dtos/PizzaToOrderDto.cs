using FoodOrderApp.Models.Enums;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderApp.Models.Dtos
{
    public class PizzaToOrderDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public SizeEnum Size { get; set; }

        [Required]
        public List<int> Ingredients { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
