using FoodOrderApp.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.Providers
{
    public interface IEmailSender : IDisposable 
    {
        /// <summary>
        /// Sends confirmation email to users email address
        /// </summary>
        /// <param name="receiver">Recipent of email</param>
        /// <param name="confirmationLink">confirmation link that will be included to message</param>
        Task SendAccountConfirmation(UserModel receiver, string confirmationLink);
    }
}
