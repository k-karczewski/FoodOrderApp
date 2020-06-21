using FoodOrderApp.Interfaces.Services.ServiceResults;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.Services
{
    public interface IFoodService
    {
        /// <summary>
        /// Removes object from databse
        /// </summary>
        /// <param name="objectId">id of object to be removed</param>
        /// <returns>Operation status (Deleted or Error)</returns>
        Task<IServiceResult> DeleteAsync(int objectId);
    }
}
