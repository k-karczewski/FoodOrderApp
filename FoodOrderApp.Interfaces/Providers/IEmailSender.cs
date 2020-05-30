using FoodOrderApp.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.Providers
{
    public interface IEmailSender : IDisposable 
    {
        Task SendAccountConfirmation(UserModel receiver, string confirmationLink);
    }
}
