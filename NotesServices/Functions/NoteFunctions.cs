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

namespace NotesServices
{
    public class NoteFunctions
    {
        private readonly INoteRL noteRL;
        private readonly IJWTService _jWTService;

        public NoteFunctions(INoteRL noteRL, IJWTService jWTService)
        {
            this.noteRL = noteRL;
            this._jWTService = jWTService;
        }
        [FunctionName("GetAllNotes")]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger Getallnotes function processed a request.");


            try
            {


                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                var result = await this.noteRL.GetAll();
                return new OkObjectResult(result);

            }
            catch (CosmosException cosmosException)
            {


                log.LogError("Getallnotes  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to create item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");


            }
        }
        [FunctionName("GetNoteById")]
        public async Task<IActionResult> GetAllNotesById(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "notes/{id}")] HttpRequest req, string id)
        {
            var authResponse = _jWTService.ValidateJWT(req);
            if (!authResponse.IsValid)
            {
                return new UnauthorizedResult();
            }
            var response = await this.noteRL.GetAllNotesById(authResponse.Email, id);
            return new OkObjectResult(response);
        }

        [FunctionName("UpdateNote")]
        public async Task<IActionResult> UpdateNote(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "note/Update/{id}")] HttpRequest req, string id)
        {
            var authResponse = _jWTService.ValidateJWT(req);
            if (!authResponse.IsValid)
            {
                return new UnauthorizedResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<NoteModel>(requestBody);

            var response = await this.noteRL.UpdateNote(authResponse.Email, data, id);

            return new OkObjectResult(response);
        }

        [FunctionName("DeleteNote")]
        public async Task<IActionResult> DeleteNote(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "note/Delete/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger DeleteNote function processed a request.");
            try
            {
                var authresponse = this._jWTService.ValidateJWT(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<NoteModel>(requestBody);

                var response = this.noteRL.DeleteNote(authresponse.Email, noteId);
                return new OkObjectResult(response);

            }
            catch (CosmosException cosmosException)
            {
                log.LogError("DeleteNote  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to DeleteNote item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }
        }

        [FunctionName("Pin")]
        public async Task<IActionResult> Pin(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "note/Pin/{id}")] HttpRequest req, string id)
        {
            var authResponse = _jWTService.ValidateJWT(req);
            if (!authResponse.IsValid)
            {
                return new UnauthorizedResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<NoteModel>(requestBody);

            var response = this.noteRL.Pin(authResponse.Email, id);

            return new OkObjectResult(response);
        }

        [FunctionName("Archive")]
        public async Task<IActionResult> Archive(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "note/Archive/{id}")] HttpRequest req, string id)
        {
            var authResponse = _jWTService.ValidateJWT(req);
            if (!authResponse.IsValid)
            {
                return new UnauthorizedResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<NoteModel>(requestBody);

            var response = this.noteRL.Archive(authResponse.Email, id);

            return new OkObjectResult(response);
        }

        [FunctionName("ChangeColour")]

        public async Task<IActionResult> ColourChange(
           [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "note/ChangeColour/{noteId}")] HttpRequest req,
           ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger ChangeColour function processed a request.");
            try
            {
                var authresponse = this._jWTService.ValidateJWT(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                var result = this.noteRL.ColourChange(data, authresponse.Email, noteId);
                return new OkObjectResult(result);

            }
            catch (CosmosException cosmosException)
            {
                log.LogError("ChangeColour  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to ChangeColour item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }

        }

        [FunctionName("Trash")]
        public async Task<IActionResult> Trash(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "note/Trash/{id}")] HttpRequest req, string id)
        {
            var authResponse = _jWTService.ValidateJWT(req);
            if (!authResponse.IsValid)
            {
                return new UnauthorizedResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<NoteModel>(requestBody);

            var response = this.noteRL.Trash(authResponse.Email, id);

            return new OkObjectResult(response);
        }


    }
}
