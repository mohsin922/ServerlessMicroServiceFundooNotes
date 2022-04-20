using Newtonsoft.Json;
using System;

namespace CommonLayer.Models.RequestModels
{
    public class UserRegModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }


        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; } 

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [JsonProperty("CreatedAt")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

    }
}
