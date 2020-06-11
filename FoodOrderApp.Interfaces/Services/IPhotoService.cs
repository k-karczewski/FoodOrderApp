using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.Services
{
    public interface IPhotoService
    {
        Task<IServiceResult> AddPizzaPhotoAsync(PhotoToCreateDto newPhoto);
        Task<IServiceResult> DeletePizzaPhotoAsync(int pizzaId);
    }
}
