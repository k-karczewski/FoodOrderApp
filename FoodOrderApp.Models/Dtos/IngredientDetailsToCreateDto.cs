using FoodOrderApp.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderApp.Models.Dtos
{
    public class IngredientDetailsToCreateDto
    {
        [Required]
        public SizeEnum Size { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
