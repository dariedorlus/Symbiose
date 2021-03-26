using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Symbiose.Mail.Models;

namespace Symbiose.Mail.Email_Delivery
{
    public interface IMailgunService : IDeliverEmail, IEnabled { }

    /// <inheritdoc />
    public class MailgunService: IMailgunService
    {
        private ILogger<MailgunService> logger;
        private MailgunConfigOptions mailgunOptions;
        private HttpClient httpClient;

        public MailgunService(ILogger<MailgunService> logger, IConfiguration config, HttpClient httpClient)
        {
            this.logger = logger;
            mailgunOptions = config.GetSection(MailgunConfigOptions.Mailgun).Get<MailgunConfigOptions>();

            httpClient.BaseAddress = new Uri(mailgunOptions.ApiBaseUrl);
            var authorization = Encoding.ASCII.GetBytes("api:" + mailgunOptions.ApiKey);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", 
                Convert.ToBase64String(authorization));
            this.httpClient = httpClient;
        }

        public async Task<bool> SendEmail(Email email)
        {
            if (email == null)
            {
                logger.LogError("The email provided is null.");
                return false; 
            }
            
            var url = string.Concat(mailgunOptions.Domain, "/", mailgunOptions.Resource);
            var encodedContent = ConvertEmailToFormEncodedContent(email);
            using var httpResponse = await httpClient.PostAsync(url, encodedContent);

            return httpResponse.IsSuccessStatusCode;
        }

        public bool IsEnabled()
        {
            return mailgunOptions.Enabled; 
        }

        private FormUrlEncodedContent ConvertEmailToFormEncodedContent(Email email)
        {
            if (email == null)
            {
                logger.LogError("Email is null and can not be encoded");
                return null;
            }

            var data = new List<KeyValuePair<string, string>>()
            {
                new("to", string.Concat(email.ToName, " <", email.To, ">")),
                new ("from", string.Concat(email.From, " <", email.From, ">")),
                new ("subject", email.Subject),
                new ("text", email.BodyText),
                new ("html", email.BodyHtml)
            };

            return new FormUrlEncodedContent(data); 
        }
    }
}
