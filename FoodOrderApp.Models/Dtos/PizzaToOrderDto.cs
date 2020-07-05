using FoodOrderApp.Models.Enums;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderApp.Models.Dtos
{
    public class PizzaToOrderDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public SizeEnum Size { get; set; }

        public List<int> AddedIngredientsIds { get; set; }
        public List<int> RemovedIngredientsIds { get; set; }
    }
}
