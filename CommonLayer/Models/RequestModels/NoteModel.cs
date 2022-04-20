using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models.RequestModels
{
    public class NoteModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string NoteBody { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPinned { get; set; }
        public bool IsArchived { get; set; }
        public string Color { get; set; }
        public string BImage { get; set; }
        public DateTime? Reminder { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public UserRegModel regModel { get; set; }
    }

}
