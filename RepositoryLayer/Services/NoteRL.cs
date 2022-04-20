using CommonLayer.Models.RequestModels;
using Microsoft.Azure.Cosmos;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Services
{
    public class NoteRL : INoteRL
    {
        private readonly CosmosClient _cosmosClient;

        public NoteRL(CosmosClient _cosmosClient)
        {
            this._cosmosClient = new CosmosClient(System.Environment.GetEnvironmentVariable("CosmosDBConnection", EnvironmentVariableTarget.Process));
        }

        public async Task<NoteModel> CreateNote(NoteModel noteModel)
        {
            if (noteModel == null)
            {
                throw new NullReferenceException();
            }
            try
            {
                var note = new NoteModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = noteModel.Title,
                    NoteBody = noteModel.NoteBody,
                    Reminder = noteModel.Reminder,
                    Color = noteModel.Color,
                    BImage = noteModel.BImage,
                    IsArchived = noteModel.IsArchived,
                    IsPinned = noteModel.IsPinned,
                    IsDeleted = noteModel.IsDeleted,
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now

                };

                var container = this._cosmosClient.GetContainer("NotesDB", "NotesContainer");
                using (var result = container.CreateItemAsync(note, new PartitionKey(note.Id.ToString())))
                {
                    return (result.Result.Resource);
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}