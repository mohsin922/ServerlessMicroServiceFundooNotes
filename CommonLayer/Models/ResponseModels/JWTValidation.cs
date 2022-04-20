using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models.ResponseModels
{
    public class JWTValidation
    {

        public bool IsValid { get; set; }

        public string Id { get; set; }

        public string Email { get; set; }
    }
}
