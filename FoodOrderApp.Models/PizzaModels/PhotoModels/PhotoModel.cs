using FoodOrderApp.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.PizzaModels.PhotoModels
{
    public class PhotoModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }

        public int PizzaId { get; set; }
        public PizzaModel Pizza { get; set; }
    }
}
