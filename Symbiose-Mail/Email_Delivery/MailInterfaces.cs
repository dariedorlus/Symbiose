using System.Threading.Tasks;
using Symbiose.Mail.Models;

namespace Symbiose.Mail.Email_Delivery
{
    public interface IDeliverEmail
    {
        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>
        ///   <c>true</c> if this email is sent; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> SendEmail(Email email);
    }

    public interface IEnabled
    {
        /// <summary>
        /// Determines whether this instance is enabled.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </returns>
        bool IsEnabled();
    }
}