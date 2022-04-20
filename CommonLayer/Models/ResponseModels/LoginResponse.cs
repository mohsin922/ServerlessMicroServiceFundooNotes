using CommonLayer.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models.ResponseModels
{
    public class LoginResponse
    {
        public UserRegModel userRegModel { get; set; }

        public string token { get; set; }
    }
}
