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

namespace UserServices.Functions
{
    public  class ForgetPasswordFunc
    {
        private IUserRL userRL;

        public ForgetPasswordFunc(IUserRL userRL)
        {
            this.userRL = userRL;
        }
        [FunctionName("ForgetPassword")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<ForgetPasswordModel>(requestBody);

                var result = this.userRL.ForgetPassword(data);
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
