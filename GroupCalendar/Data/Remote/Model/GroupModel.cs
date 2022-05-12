using GroupCalendar.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GroupCalendar.Data.Remote.Model
{
    public partial class GroupModel : ObservableObject
    {
        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("events")]
        public List<EventModel> Events { get; set; } = new List<EventModel>();

        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonProperty("image")]
        public Uri Image { get; set; } = new Uri("https://uning.es/wp-content/uploads/2016/08/ef3-placeholder-image.jpg");

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("admins")]
        public List<string> Admins { get; set; } = new List<string>();

        [JsonProperty("users")]
        public List<string> Users { get; set; } = new List<string>();

        [JsonIgnore]
        private bool currentUserIsAdmin;

        [JsonIgnore]
        public bool CurrentUserIsAdmin
        {
            get { return currentUserIsAdmin; }
            set { currentUserIsAdmin = value; OnPropertyChanged(); }
        }

    }

}

