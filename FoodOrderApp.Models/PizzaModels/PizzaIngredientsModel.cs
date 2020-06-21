namespace FoodOrderApp.Models.PizzaModels
{
    public class PizzaIngredientsModel
    {
        public int IngredientId { get; set; }
        public IngredientModel Ingredient { get; set; }

        public int PizzaId { get; set; }
        public PizzaModel Pizza { get; set; }
    }
}
