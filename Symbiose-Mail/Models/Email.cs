using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Symbiose.Mail.Models
{
    
    public class Email: EntityBase
    {
        [Required(ErrorMessage = "The 'to' field is required")]
        [EmailAddress(ErrorMessage = "The 'to' email is not valid" )]
        public string To { get; set; }
        
        [Required(ErrorMessage = "The 'to_name' field is required")]
        public string ToName { get; set; }
        
        [Required(ErrorMessage = "The 'from' field is required")]
        [EmailAddress(ErrorMessage = "The 'from' email is not valid")]
        public string From { get; set; }
        
        [Required(ErrorMessage = "The 'from_name' field is required")] 
        public string FromName { get; set; }

        [Required(ErrorMessage = "The 'subject' field is required")] 
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the html version of the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        [JsonPropertyName("body")]
        [Required(ErrorMessage = "The 'body' field is required")] 
        public string BodyHtml { get; set; }

        /// <summary>
        /// Gets or sets the text version of the body.
        /// </summary>
        /// <value>
        /// The body text.
        /// </value>
        [JsonIgnore]
        public string BodyText { get; set; }
        
        [JsonIgnore]
        public bool IsSent { get; set; }
    }
}