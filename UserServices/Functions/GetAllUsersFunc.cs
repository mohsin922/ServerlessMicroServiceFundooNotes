using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RepositoryLayer.Interfaces;

namespace UserServices.Functions
{
    public class GetAllUsersFunc
    {
        private IUserRL userRL;

        public GetAllUsersFunc(IUserRL userRL)
        {
            this.userRL = userRL;
        }
        [FunctionName("GetAllUsers")]
        public async Task<IActionResult> GetUsers(
         [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetAll")] HttpRequest req,
         ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            var response = await this.userRL.GetUsers();

            return new OkObjectResult(response);

        }


    }
}
