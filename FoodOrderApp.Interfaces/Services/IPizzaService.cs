﻿using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.PizzaModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.Services
{
    public interface IPizzaService : IFoodService
    {

        /// <summary>
        /// Gets all pizza objects from database
        /// </summary>
        /// <returns>List of PizzaToReturnDto or list of errors (depending on returned ServiceResult state)</returns>
        Task<IServiceResult<List<PizzaToReturnDto>>> GetAsync();

        /// <summary>
        /// Pizza stored with name passed by parameter
        /// </summary>
        /// <param name="name">name of pizza</param>
        /// <returns>Pizza to return object with specific name</returns>
        Task<IServiceResult<PizzaToReturnDto>> GetByNameAsync(string name);

        /// <summary>
        /// Creates new pizza in database
        /// </summary>
        /// <param name="pizzaToCreate">data of pizza to be created</param>
        /// <returns>Created pizza or errors that occured during creation (depending on returned ServiceResult state)</returns>
        Task<IServiceResult<PizzaToReturnDto>> CreateAsync(PizzaToCreateDto pizzaToCreate);

        /// <summary>
        /// Updates total price of pizza. The total price is based on stater price and included ingredients prices.
        /// </summary>
        /// <param name="pizzaId"></param>
        /// <returns>Updated pizza model</returns>
        Task<IServiceResult<PizzaModel>> UpdatePriceAsync(int pizzaId);

        /// <summary>
        /// Adds ingredient to existing pizza
        /// </summary>
        /// <param name="pizzaName">name of pizza that ingredient will be added for</param>
        /// <param name="ingredientId">identifier of ingredient</param>
        /// <returns>PizzaToReturn object or list of errors</returns>
        Task<IServiceResult<PizzaToReturnDto>> AddIngredientAsync(string pizzaName, int ingredientId);

        /// <summary>
        /// Deletes ingredient from existing pizza
        /// </summary>
        /// <param name="pizzaName">name of pizza that ingredient will be removed from</param>
        /// <param name="ingredientId">identifier of ingredient</param>
        /// <returns>PizzaToReturn object or list of errors</returns>
        Task<IServiceResult<PizzaToReturnDto>> DeleteIngredientAsync(string pizzaName, int ingredientId);

        /// <summary>
        /// Gets Pizza object with specific id
        /// </summary>
        /// <param name="id">id of pizza</param>
        /// <returns>IServiceResult with operation status, T objects from database</returns>
        Task<IServiceResult<PizzaToReturnDto>> GetByIdAsync(int id);
    }
}
