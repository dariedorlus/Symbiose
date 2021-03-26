using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Symbiose.Mail.Models;

namespace Symbiose.Mail.Email_Delivery
{
    public interface ISendgridService : IDeliverEmail, IEnabled { }

    public class SendgridService: ISendgridService
    {
        private ILogger<SendgridService> logger;
        private SendgridConfigOptions sendgridConfigOptions;
        private HttpClient httpClient;

        public SendgridService(ILogger<SendgridService> logger, IConfiguration config, HttpClient httpClient)
        {
            this.logger = logger;
            sendgridConfigOptions = config.GetSection(SendgridConfigOptions.Sendgrid).Get<SendgridConfigOptions>();
            httpClient.BaseAddress = new Uri(sendgridConfigOptions.ApiBaseUrl);
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", sendgridConfigOptions.ApiKey);

           this.httpClient = httpClient;
        }

        public async Task<bool> SendEmail(Email email)
        {
            if (email == null)
            {
                logger.LogError("The email provided is null.");
                return false;
            }

            var sendgridContent = ConvertEmailToSendgridContent(email);
            using var httpResponse = await httpClient.PostAsJsonAsync(sendgridConfigOptions.Resource, sendgridContent);

            return httpResponse.IsSuccessStatusCode;
        }
        public bool IsEnabled()
        {
            return sendgridConfigOptions.Enabled;
        }

        /// <summary>
        /// Converts the content of the email to sendgrid DTO.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        private object ConvertEmailToSendgridContent(Email email)
        {
            var sendgridContent = new
            {
                Personalizations = new[]
                {
                    new
                    {
                        To = new[]
                        {
                            new
                            {
                                Email = email.To,
                                Name = email.ToName
                            }
                        },
                        Subject = email.Subject
                    }
                },
                Content = new[]
                {
                    new
                    {
                        Type = "text/plain",
                        Value = email.BodyText
                    },
                    new
                    {
                        Type = "text/html",
                        Value = email.BodyHtml
                    }
                },
                From = new
                {
                    Email = email.From,
                    Name = email.FromName
                }
            };
            return sendgridContent;
        }

    }
}
