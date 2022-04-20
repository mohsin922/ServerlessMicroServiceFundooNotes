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
using CommonLayer.Models.RequestModels;
using Microsoft.Azure.Cosmos;

namespace UserServices
{
    public class UserRegistrationFunc
    {
        private IUserRL userRL;

        public UserRegistrationFunc(IUserRL userRL)
        {
            this.userRL = userRL;
        }

        [FunctionName("UserRegistration")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger UserRegistration function processed a request.");

            try
            {


                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<UserRegModel>(requestBody);

                var result = await this.userRL.CreateUser(data);
                return new OkObjectResult(result);

            }
            catch (CosmosException cosmosException)
            {


                log.LogError("Creating item failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to create item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");


            }

        }
    }
}
