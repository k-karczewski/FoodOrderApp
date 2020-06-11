using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodOrderApp.Models.Dtos
{
    public class PhotoToCreateDto
    {
        [Required]
        public byte[] PhotoInBytes { get; set; }
        [Required]
        public int PizzaId { get; set; }
    }
}
