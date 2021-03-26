using System;
using System.Text.Json.Serialization;

namespace Symbiose.Mail.Models
{
    public class EntityBase
    {
        
        private string id;
        [JsonIgnore]
        public string Id
        {
            get => (string.IsNullOrEmpty(id) ? id = Guid.NewGuid().ToString() : id);

            set => id = value;
        }
        
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public DateTime UpdatedDate { get; set; }
    }
}