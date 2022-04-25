using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models.RequestModels
{
    public class NoteModel
    {
        [JsonProperty("id")]
        public string NoteId { get; set; } = string.Empty;

        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("notebody")]
        public string NoteBody { get; set; } = string.Empty;

        [JsonProperty("color")]
        public string Color { get; set; } = string.Empty;

        [JsonProperty("isPinned")]
        public bool IsPinned { get; set; } = false;

        [JsonProperty("isArchived")]
        public bool IsArchived { get; set; } = false;

        [JsonProperty("isTrash")]
        public bool IsTrash { get; set; } = false;

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; } = string.Empty;
    }

}
