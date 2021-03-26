namespace Symbiose.Mail.Email_Delivery
{
    public class MailgunConfigOptions: MailConfigOptionsBase
    {
        public const string Mailgun = "Mailgun";

        /// <summary>
        /// Gets or sets the domain on which emails will be sent on.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        public string Domain { get; set; }


       
    }
}