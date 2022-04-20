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
using Microsoft.Azure.Cosmos;
using CommonLayer.Models.RequestModels;
using System.Collections.Generic;
using System.Linq;

namespace UserServices.Functions
{
    public class UserLoginFunc
    {
        private IUserRL userRL;

        public UserLoginFunc(IUserRL userRL)
        {
            this.userRL = userRL;
        }
        [FunctionName("Login")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<UserLogin>(requestBody);
                var result = this.userRL.Login(data);
                if (result != null)
                {
                    return new OkObjectResult(result);
                }
                return new BadRequestResult();
            }
            catch (CosmosException cosmosException)
            {

                log.LogError(" forget password failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to proced for forget password Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }
        }
       

    }
    
}

