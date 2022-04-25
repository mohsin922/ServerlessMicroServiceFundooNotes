using CommonLayer.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interfaces
{
    public interface INoteRL
    {
        Task<NoteModel> CreateNote(string email,NoteModel noteModel);
        Task<List<NoteModel>> GetAll();
        Task<List<NoteModel>> GetAllNotesById(string email, string id);
        Task<NoteModel> UpdateNote(string email, NoteModel updateNote, string noteId);
        Task<bool> DeleteNote(string email, string noteId);

        bool Pin(string email, string noteId);

        bool Archive(string email, string noteId);
        Task<bool> ColourChange(string colour, string email, string noteId);

        bool Trash(string email, string noteId);
    }
}
