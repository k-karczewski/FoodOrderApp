using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.OrderModels;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.UserModels;
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





                }






            }catch(Exception) { }










            throw new NotImplementedException();
        }






        public Task<IServiceResult> CancelOrder(int orderId)
        {
            throw new NotImplementedException();
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

        //private ICollection<PizzaOrderModel> ConvertToOrder(IEnumerable<PizzaToOrderDto> pizzasToOrder)
        //{
        //    List<PizzaOrderModel> order = new List<PizzaOrderModel>();

        //    foreach (PizzaToOrderDto dto in pizzasToOrder)
        //    {
        //        dto.
        //        order.Add(new PizzaOrderModel
        //        {
        //            i
        //        });
        //    }
        //}

    }
}
