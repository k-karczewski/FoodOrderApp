using FoodOrderApp.Interfaces.Providers;
using FoodOrderApp.Models.UserModels;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Providers
{
    public class EmailSender : IEmailSender
    {
        private IConfiguration _configuration;

        public EmailSender(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public void Dispose()
        {
            _configuration = null;
        }

        /// <summary>
        /// Sends confirmation email to users email address
        /// </summary>
        /// <param name="receiver">Recipent of email</param>
        /// <param name="confirmationLink">confirmation link that will be included to message</param>
        public virtual async Task SendAccountConfirmation(UserModel receiver, string confirmationLink)
        {
            SendGridMessage message = new SendGridMessage
            {
                From = new EmailAddress("FoodOrder@app.com", "FoodOrderApp"),
                Subject = "Confirm your account @ FoodOrderApp",
                HtmlContent = $"<a href={confirmationLink}>Click here to confirm your account</a>",
            };

            var recipent = new EmailAddress(receiver.Email, receiver.UserName);
            message.AddTo(recipent);

            SendGridClient client = new SendGridClient(_configuration.GetSection("SendGridKeys:DefaultKey").Value);

            await client.SendEmailAsync(message);
        }
    }
}
