using CommonLayer.Models.RequestModels;
using Microsoft.Azure.Cosmos;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public async Task<NoteModel> CreateNote(string email, NoteModel noteModel)
        {
            if (noteModel == null)
            {
                throw new NullReferenceException();
            }
            try
            {
                var note = new NoteModel()
                {
                    NoteId = Guid.NewGuid().ToString(),
                    Title = noteModel.Title,
                    NoteBody = noteModel.NoteBody,
                    Color = noteModel.Color,
                    IsArchived = noteModel.IsArchived,
                    IsPinned = noteModel.IsPinned,
                    IsTrash = noteModel.IsTrash,
                    CreatedAt = Convert.ToString(DateTime.Now),

                };

                var container = this._cosmosClient.GetContainer("NotesDB", "NotesContainer");
                using (var result = container.CreateItemAsync(note, new PartitionKey(note.NoteId.ToString())))
                {
                    return (result.Result.Resource);
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public async Task<List<NoteModel>> GetAll()
        {
            try
            {
                var sqlQueryText = "SELECT * FROM NotesContainer ";

                QueryDefinition query = new QueryDefinition(sqlQueryText);
                var container = this._cosmosClient.GetContainer("NotesDB", "NotesContainer");
                using FeedIterator<NoteModel> queryResultSetIterator = container.GetItemQueryIterator<NoteModel>(query);

                List<NoteModel> notes = new List<NoteModel>();

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<NoteModel> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (var item in currentResultSet)
                    {
                        notes.Add(item);
                    }

                    return notes;

                }
                return notes;
            }
            catch (CosmosException ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public async Task<List<NoteModel>> GetAllNotesById(string email, string id)
        {
            List<NoteModel> notes = new List<NoteModel>();
            var container = this._cosmosClient.GetContainer("NotesDB", "NotesContainer");
            try
            {
                NoteModel note = await container.ReadItemAsync<NoteModel>(id, new PartitionKey(id));
                if (note != null)
                {
                    notes.Add(note);
                }
                return notes;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<NoteModel> UpdateNote(string email, NoteModel updateNote, string noteId)
        {
            try
            {
                var container = this._cosmosClient.GetContainer("NotesDB", "NotesContainer");
                var update = container.GetItemLinqQueryable<NoteModel>(true).Where(b => b.NoteId == noteId)
                        .AsEnumerable().FirstOrDefault();

                if (update != null)
                {
                    update.Title = updateNote.Title;
                    update.NoteBody = updateNote.NoteBody;
                    update.Color = updateNote.Color;

                    var response = await container.ReplaceItemAsync<NoteModel>(update, update.NoteId, new PartitionKey(update.NoteId));
                    return response.Resource;

                }
                return null;

            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<bool> DeleteNote(string email, string noteId)
        {
            var container = this._cosmosClient.GetContainer("NotesDB", "NotesContainer");
            var document = container.GetItemLinqQueryable<NoteModel>(true).Where(b => b.NoteId == noteId)
                            .AsEnumerable().FirstOrDefault();

            if (document.IsTrash == true)
            {
                using (ResponseMessage response = await container.DeleteItemStreamAsync(noteId, new PartitionKey(noteId)))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;

        }

        public bool Pin(string email, string noteId)
        {
            var container = this._cosmosClient.GetContainer("NotesDB", "NotesContainer");
            var document = container.GetItemLinqQueryable<NoteModel>(true).Where(b => b.NoteId == noteId)
                            .AsEnumerable().FirstOrDefault();

            if (document != null)
            {
                if (document.IsPinned == true)
                {
                    document.IsPinned = false;
                }
                else
                {
                    document.IsPinned = true;
                }
                container.ReplaceItemAsync<NoteModel>(document, document.NoteId, new PartitionKey(document.NoteId));
                return true;
            }
            return false;
        }

        public bool Archive(string email, string noteId)
        {
            var container = this._cosmosClient.GetContainer("NotesDB", "NotesContainer");
            var document = container.GetItemLinqQueryable<NoteModel>(true).Where(b => b.NoteId == noteId)
                            .AsEnumerable().FirstOrDefault();

            if (document != null)
            {
                if (document.IsArchived == true)
                {
                    document.IsArchived = false;
                }
                else
                {
                    document.IsArchived = true;
                }
                container.ReplaceItemAsync<NoteModel>(document, document.NoteId, new PartitionKey(document.NoteId));
                return true;
            }
            return false;
        }

        public async Task<bool> ColourChange(string color,string email,string noteId)
        {
            if (noteId == null)
            {
                throw new Exception("please pass userId and noteId compulsary ");
            }
            if (color == null)
            {
                throw new Exception("Please pass colour to change ");
            }

            try
            {
                var container = this._cosmosClient.GetContainer("NotesDB", "NotesContainer");
                var document = container.GetItemLinqQueryable<NoteModel>(true)
                               .Where(b => b.NoteId == noteId)
                               .AsEnumerable()
                               .FirstOrDefault();
                if (document != null)
                {
                    ItemResponse<NoteModel> response = await container.ReadItemAsync<NoteModel>(document.NoteId, new PartitionKey(document.NoteId));
                    var itembody = response.Resource;
                    itembody.Color = color;
                    response = await container.ReplaceItemAsync<NoteModel>(itembody, itembody.NoteId, new PartitionKey(itembody.NoteId));
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Trash(string email, string noteId)
        {
            var container = this._cosmosClient.GetContainer("NotesDB", "NotesContainer");
            var document = container.GetItemLinqQueryable<NoteModel>(true).Where(b => b.NoteId == noteId)
                            .AsEnumerable().FirstOrDefault();

            if (document != null)
            {
                if (document.IsTrash == true)
                {
                    document.IsTrash = false;
                }
                else
                {
                    document.IsTrash = true;
                }
                container.ReplaceItemAsync<NoteModel>(document, document.NoteId, new PartitionKey(document.NoteId));
                return true;
            }
            return false;
        }
    }
}