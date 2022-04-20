using CommonLayer.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interfaces
{
    public interface IJWTService
    {
        string GetJWT(string id, string email);

        JWTValidation ValidateJWT(HttpRequest req);
    }
}
