using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Models.PizzaModels.PriceModels
{
    public class PizzaPriceModel : PriceModel
    {
        public int PizzaId { get; set; }
        public PizzaModel Pizza { get; set; }
    }
}
