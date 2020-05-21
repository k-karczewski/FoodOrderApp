using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.PizzaModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.Services
{
    public interface IPizzaService : IFoodService<PizzaModel>
    {
        /// <summary>
        /// Creates new pizza in database
        /// </summary>
        /// <param name="newPizza">data of pizza to be created</param>
        /// <returns>List of created pizzas or errors (depending on returned ServiceResult state)</returns>
        Task<IServiceResult<List<PizzaModel>>> CreateAsync(PizzaToCreateDto newPizza);

        /// <summary>
        /// Gets all pizza objects from database
        /// </summary>
        /// <returns>List of PizzaToReturnDto or list of errors (depending on returned ServiceResult state)</returns>
        Task<IServiceResult<List<PizzaToReturnDto>>> GetAsync();

        /// <summary>
        /// Updates total price of pizza. The total price is based on stater price and included ingredients prices.
        /// </summary>
        /// <param name="pizzaId"></param>
        /// <returns>Updated pizza model</returns>
        Task<IServiceResult<PizzaModel>> UpdatePriceAsync(int pizzaId);

        /// <summary>
        /// Gets all pizzas with name passed by parameter
        /// </summary>
        /// <param name="name">name of pizza</param>
        /// <returns>List of pizzas with specific name</returns>
        Task<IServiceResult<List<PizzaToReturnDto>>> GetByName(string name);
    }
}
