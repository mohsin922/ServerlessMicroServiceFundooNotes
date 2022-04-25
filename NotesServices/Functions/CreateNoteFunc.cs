using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CommonLayer.Models.RequestModels;
using Microsoft.Azure.Cosmos;
using RepositoryLayer.Interfaces;

namespace NotesServices.Functions
{
    public class CreateNoteFunc
    {
        private readonly INoteRL noteRL;
        private readonly IJWTService _jWTService;

        public CreateNoteFunc(INoteRL noteRL, IJWTService jWTService)
        {
            this.noteRL = noteRL;
            this._jWTService = jWTService;
        }

        [FunctionName("CreateNote")]
        public async Task<IActionResult> CreateNote(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "note/CreateNote")] HttpRequest req)
        {
            //ValidateJWT auth = new ValidateJWT(req);
            var authResponse = _jWTService.ValidateJWT(req);
            if (!authResponse.IsValid)
            {
                return new UnauthorizedResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //string requestBody = String.Empty;
            //using (StreamReader streamReader = new StreamReader(req.Body))
            //{
            //    requestBody = await streamReader.ReadToEndAsync();
            //}
            dynamic data = JsonConvert.DeserializeObject<NoteModel>(requestBody);

            var response = await this.noteRL.CreateNote(authResponse.Email, data );
            return new OkObjectResult(response);
        }

    }
    
}
