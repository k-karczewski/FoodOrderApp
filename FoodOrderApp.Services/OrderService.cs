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
using System.Text;
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

        public async Task<IServiceResult> MakeOrder(ICollection<PizzaToOrderDto> pizzas, int userId)
        {
            try
            {
                List<int> pizzaIds = pizzas.Select(x => x.PizzaId).ToList();

                if(await CheckIfPizzasExist(pizzaIds))
                {
                    UserModel purchaser = (await _repository.Users.GetByExpressionAsync(x => x.Id == userId, i => i.Include(o => o.Orders))).SingleOrDefault();
                    List<PizzaOrderModel> pizzaOrder = await ConvertToOrder(pizzas);

                    OrderModel order = new OrderModel
                    {
                        UserId = purchaser.Id,
                        User = purchaser,
                        PizzaOrders = pizzaOrder,
                        Status = OrderStatus.New,
                        TotalPrice = CountTotalOrderPrice(pizzaOrder)
                    };

                    await _repository.Orders.CreateAsync(order);
                    await _repository.SaveChangesAsync();

                    return new ServiceResult(ResultType.Correct);
                }
                else
                {
                    throw new Exception("Error during creating order");
                }
            }
            catch(Exception e) 
            {
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }
        }

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

                throw new Exception("Order does not exist or wrong user was chosen");

            }
            catch(Exception e)
            {
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }
        }

        private async Task<bool> CheckIfPizzasExist(IEnumerable<int> pizzaIds)
        {
            bool doExist = true;

            foreach(int id in pizzaIds)
            {
                PizzaModel p = (await _repository.Pizzas.GetByExpressionAsync(x => x.Id == id)).SingleOrDefault();

                if(p == null)
                {
                    doExist = false;
                    break;
                }
            }

            return doExist;
        }

        private async Task<List<PizzaOrderModel>> ConvertToOrder(IEnumerable<PizzaToOrderDto> orderItems)
        {
            List<PizzaOrderModel> order = new List<PizzaOrderModel>();

            try
            {
                foreach (PizzaToOrderDto dto in orderItems)
                {
                    PizzaModel pizzaToOrder = (await _repository.Pizzas.GetByExpressionAsync(x => x.Id == dto.PizzaId, i => i.Include(d => d.PizzaDetails))).SingleOrDefault();
                    PizzaDetailsModel pizzaDetail = pizzaToOrder.PizzaDetails.FirstOrDefault(x => x.Size == dto.Size);
                    pizzaToOrder.PizzaDetails = new List<PizzaDetailsModel>();
                    pizzaToOrder.PizzaDetails.Add(pizzaDetail);

                    if (pizzaToOrder != null)
                    {
                        order.Add(new PizzaOrderModel
                        {
                            PizzaDetailId = pizzaToOrder.PizzaDetails.FirstOrDefault().Id,
                            PizzaDetail = pizzaToOrder.PizzaDetails.FirstOrDefault(),
                            PizzaId = pizzaToOrder.Id
                        });
                    }
                    else
                    {
                        return null;
                    }
                }

                return order;
            }
            catch(Exception)
            {
                return null;
            }
        }

        private decimal CountTotalOrderPrice(List<PizzaOrderModel> orderItems)
        {
            decimal total = 0;

            foreach(PizzaOrderModel item in orderItems)
            {
                total += item.PizzaDetail.TotalPrice;
            }

            return total;
        }
    }
}
