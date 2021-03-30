using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Symbiose.Mail.Email_Delivery;
using Symbiose.Mail.Models;
using Symbiose.Mail.Repositories;

namespace Symbiose.Mail.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Gets the email by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The email requested</returns>
        Task<Email> GetEmailById(string id);
        /// <summary>
        /// Processes the email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        Task<Email> ProcessEmail(Email email);

        /// <summary>
        /// Gets all emails.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Email>> GetAllEmails();
    }

    ///<inheritdoc cref="IEmailService"/>
    public class EmailService : IEmailService
    {
        private IEmailRepository repo;
        private IMailgunService mailgunService;
        private ISendgridService sendgridService;
        private ILogger<EmailService> logger;

        public EmailService(IEmailRepository repo, ILogger<EmailService> logger, IMailgunService mailgunService, ISendgridService sendgridService)
        {
            this.repo = repo;
            this.logger = logger;
            this.mailgunService = mailgunService;
            this.sendgridService = sendgridService;
        }

        public async Task<Email> GetEmailById(string id)
        {
            return await repo.GetOneById(id);
        }

        public async Task<Email> ProcessEmail(Email email)
        {
            if (!IsEmailValid(email))
            {
                return null;
            }

            email.BodyText = ConvertHtmlToText(email.BodyHtml);
            email.IsSent = await mailgunService.SendEmail(email);

            if (!email.IsSent)
            {
                email.IsSent = await sendgridService.SendEmail(email);
            }

            email.CreatedDate = email.UpdatedDate = DateTime.Now;

            return await repo.InsertOneEntity(email);

        }



        public async Task<IEnumerable<Email>> GetAllEmails()
        {
            var res = await repo.GetAll();
            return res.AsEnumerable();
        }

        private string ConvertHtmlToText(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                logger.LogError("Empty or null html string provided.");
                return null;
            }

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var sb = new StringBuilder();

            foreach (var node in htmlDoc.DocumentNode.ChildNodes)
            {
                sb.Append(node.InnerText);
            }

            return sb.ToString();
        }

        private bool IsEmailValid(Email email)
        {
            if (email == null)
            {
                logger.LogError("Email provided is null.");
                return false;
            }


            if (string.IsNullOrEmpty(email.To) || string.IsNullOrEmpty(email.ToName))
            {
                logger.LogError("Empty or null To/ToName string provided.");
                return false;
            }

            if (string.IsNullOrEmpty(email.From) || string.IsNullOrEmpty(email.FromName))
            {
                logger.LogError("Empty or null From/FromName string provided.");
                return false;
            }

            if (string.IsNullOrEmpty(email.BodyHtml))
            {
                logger.LogError("Empty or null html string provided.");
                return false;
            }

            return true;
        }
    }
}