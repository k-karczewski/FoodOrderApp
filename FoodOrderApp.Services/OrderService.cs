using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.Enums;
using FoodOrderApp.Models.OrderModels;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.DetailModels;
using FoodOrderApp.Models.UserModels;
using FoodOrderApp.Services.ServiceResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _repository;

        public OrderService(IUnitOfWork repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Creates new order
        /// </summary>
        /// <param name="pizzasToOrder">List of pizzas to order</param>
        /// <param name="userId">id of user that makes order</param>
        /// <returns>ServiceResult of statuses correct or error</returns>
        public async Task<IServiceResult> MakeOrder(List<PizzaToOrderDto> pizzasToOrder, int userId)
        {
            try
            {
                UserModel purchaser = (await _repository.Users.GetByExpressionAsync(x => x.Id == userId)).SingleOrDefault();

                if(purchaser != null)
                {
                    List<OrderItemModel> orderItems = await ConvertToOrderItems(pizzasToOrder);

                    if(orderItems != null)
                    {
                        OrderModel order = new OrderModel
                        {
                            UserId = purchaser.Id,
                            User = purchaser,
                            OrderItems = orderItems,
                            Status = OrderStatus.New
                        };

                        await _repository.Orders.CreateAsync(order);
                        await _repository.SaveChangesAsync();

                        return new ServiceResult(ResultType.Correct);
                    }
                }

                return new ServiceResult(ResultType.Error, new List<string> { "Error during creation of order" });
            }
            catch(Exception e) 
            {
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Cancels order
        /// </summary>
        /// <param name="orderId">id of order to be canceled</param>
        /// <param name="userId">id of user that order belongs to</param>
        /// <returns>ServiceResult of statuses edited or error</returns>
        public async Task<IServiceResult> CancelOrder(int orderId, int userId)
        {
            try
            {
                OrderModel orderToCancel = (await _repository.Orders.GetByExpressionAsync(x => x.Id == orderId)).SingleOrDefault();

                if(orderToCancel != null && orderToCancel.UserId == userId)
                {
                    orderToCancel.Status = OrderStatus.Canceled;
                    _repository.Orders.Update(orderToCancel);
                    await _repository.SaveChangesAsync();

                    return new ServiceResult(ResultType.Edited);
                }

                return new ServiceResult(ResultType.Error, new List<string> { "Error during order cancel operation" });
            }
            catch(Exception e)
            {
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Deletes order from database
        /// </summary>
        /// <param name="orderId">id of order to be deleted</param>
        /// <returns>ServiceResult of statuses deleted or error</returns>
        public async Task<IServiceResult> DeleteOrder(int orderId)
        {
            try
            {
                OrderModel orderToDelete = (await _repository.Orders.GetByExpressionAsync(x => x.Id == orderId, i => i.Include(po => po.OrderItems))).SingleOrDefault();

                // order can be deleted only by user with admin privileges so check of userId is not needed
                if (orderToDelete != null)
                {
                    _repository.Orders.Delete(orderToDelete);
                    await _repository.SaveChangesAsync();

                    return new ServiceResult(ResultType.Deleted);
                }

                return new ServiceResult(ResultType.Error, new List<string> { $"Order with id {orderId} has not been found" });
            }
            catch(Exception e)
            {
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }
        }

        #region PrivateMethods
        private async Task<List<OrderItemModel>> ConvertToOrderItems(List<PizzaToOrderDto> orderItemsDtos)
        {
            List<OrderItemModel> orderItems = new List<OrderItemModel>();

            try
            {
                foreach (PizzaToOrderDto item in orderItemsDtos)
                {
                    OrderItemModel orderItem = new OrderItemModel
                    {
                        Name = item.Name,
                        Size = item.Size,
                        OrderItemIngredients = new List<OrderIngredientModel>()
                    };

                    PizzaModel pizzaToOrder = (await _repository.Pizzas.GetByExpressionAsync(x => x.Name == item.Name, 
                            i => i.Include(pi => pi.PizzaIngredients).ThenInclude(i => i.Ingredient))).SingleOrDefault();

                    List<int> ingredientsIds = new List<int>();

                    if (pizzaToOrder != null)
                    {
                        List<IngredientModel> ingredients = pizzaToOrder.PizzaIngredients.Select(x => x.Ingredient).ToList();
                        ingredientsIds.AddRange(ingredients.Select(x => x.Id).ToList());

                        if (item.RemovedIngredientsIds != null)
                        {
                            RemoveIngredients(ref ingredientsIds, item.RemovedIngredientsIds);
                        }

                        if (item.AddedIngredientsIds != null)
                        {
                            AddIngredients(ref ingredientsIds, item.AddedIngredientsIds);
                        }
                    }
                    else if (item.AddedIngredientsIds != null)
                    {
                        ingredientsIds.AddRange(item.AddedIngredientsIds);
                    }
                    else
                    {
                        return null;
                    }

                    foreach (int ingredientId in ingredientsIds)
                    {
                        orderItem.OrderItemIngredients.Add(new OrderIngredientModel
                        {
                            IngredientId = ingredientId
                        });
                    }

                    orderItem.Price = await CalculateOrderItemPrice(orderItem.OrderItemIngredients.Select(x => x.IngredientId).ToList(), orderItem.Size);
                    orderItems.Add(orderItem);
                }
            
                return orderItems;
            }
            catch(Exception)
            {
                return null;
            }
        }

        private async Task<decimal> CalculateOrderItemPrice(List<int> orderItemIngredientsIds, SizeEnum size)
        {
            try
            {
                decimal total = ((await _repository.Starters.GetByExpressionAsync(x => x.Size == size)).Single()).Price;

                foreach(int id in orderItemIngredientsIds)
                {
                    IngredientModel i = (await _repository.Ingredients.GetByExpressionAsync(x => x.Id == id, i => i.Include(d => d.IngredientDetails))).Single();
                    total += i.IngredientDetails.FirstOrDefault(x => x.Size == size).Price;
                }

                return total;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void AddIngredients(ref List<int> ingredientList, List<int> ingredientsToAdd)
        {
            foreach (int ingredient in ingredientsToAdd)
            {
                if (ingredientList.Where(x => x == ingredient) == null)
                {
                    ingredientList.Add(ingredient);
                }
            }
        }

        private void RemoveIngredients(ref List<int> ingredientList, List<int> ingredientsToRemove)
        {
            foreach (int ingredient in ingredientsToRemove)
            {
                if (ingredientList.Where(x => x == ingredient) != null)
                {
                    ingredientList.Remove(ingredient);
                }
            }
        }
        #endregion
    }
}
