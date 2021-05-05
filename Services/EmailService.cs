using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_CA.Services
{
    public interface IEmailService
    {
        Task<Response> SendEmailAsync(List<string> emails, string subject, string message);
    }

    public class EmailService : IEmailService
    {
        public IConfiguration Configuration { get; }

        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<Response> SendEmailAsync(List<string> emails, string subject, string message)
        {
            return await ExecuteEmail(Configuration["Secret"], subject, message, emails);
        }

        public async Task<Response> ExecuteEmail(string apiKey, string subject, string message, List<string> emails)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("aze.adrarit@gmail.com", "email"),
                Subject = subject
            };

            msg.SetTemplateId(Configuration.GetSection("Sendgrid").GetSection("TemplateID").Value);
            msg.SetTemplateData(new SendgridForgotPassword
            {
                Password = message
            });

            foreach (var email in emails)
            {
                msg.AddTo(new EmailAddress(email));
            }

            Response response = await client.SendEmailAsync(msg);
            return response;
        }

        private class SendgridForgotPassword
        {
            [JsonProperty("password")]
            public string Password { get; set; }
        }
    }
}
