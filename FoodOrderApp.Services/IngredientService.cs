using FoodOrderApp.Interfaces.Repositories;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PriceModels;
using FoodOrderApp.Services.ServiceResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderApp.Services
{
    public class IngredientService : FoodServiceBase<IngredientModel>
    {
        public IngredientService(IFoodOrderRepository<IngredientModel> repository) : base(repository) { }

        public override async Task<IServiceResult<List<IngredientModel>>> GetAsync()
        {
            try
            {
                List<IngredientModel> ingredients = (await _repository.GetByExpressionAsync(x => x.Id > 0, i => i.Include(p => p.Prices))).ToList();

                return new ServiceResult<List<IngredientModel>>(ResultType.Correct, ingredients);
            }
            catch(Exception e)
            {
                return new ServiceResult<List<IngredientModel>>(ResultType.Error, new List<string> { e.Message });
            }
        }

        public override async Task<IServiceResult<IngredientModel>> GetByIdAsync(int id)
        {
            try
            {
                IngredientModel result = (await _repository.GetByExpressionAsync(x => x.Id == id, i => i.Include(p => p.Prices))).SingleOrDefault();

                if (result != null)
                    return new ServiceResult<IngredientModel>(ResultType.Correct, result);

                throw new Exception($"Object with id: {id} has not been found");
            }
            catch (Exception e)
            {
                return new ServiceResult<IngredientModel>(ResultType.Error, new List<string>() { e.Message });
            }
        }

        public override async Task<IServiceResult<IngredientModel>> UpdatePriceAsync(PriceModel price, int ingredientId)
        {
            try
            {
                IngredientModel ingredientToUpdate = (await _repository.GetByExpressionAsync(x => x.Id == ingredientId, i => i.Include(p => p.Prices))).SingleOrDefault();

                IngredientPriceModel priceToUpdate = ingredientToUpdate.Prices.SingleOrDefault(x => x.Size == price.Size);

                if(priceToUpdate != null)
                {
                    priceToUpdate.Price = price.Price;

                    await _repository.UpdateAsync(ingredientToUpdate);

                    return new ServiceResult<IngredientModel>(ResultType.Edited, ingredientToUpdate);
                }
                else
                {
                    throw new Exception("Price has not been found");
                }
            }
            catch(Exception e)
            {
                return new ServiceResult<IngredientModel>(ResultType.Error, new List<string> { e.Message });
            }

        }
    }
}
