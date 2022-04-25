using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models.RequestModels
{
    public class ForgetPasswordModel
    {
        [JsonProperty("email")]
        public string Email { get; set; } 
    }
}
