using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GroupCalendar.Data.Remote.Model
{
    public partial class GroupModel
    {
        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("events")]
        public List<EventModel> Events { get; set; } = new List<EventModel>();

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("image")]
        public Uri Image { get; set; } = new Uri("https://uning.es/wp-content/uploads/2016/08/ef3-placeholder-image.jpg");

        [JsonProperty("name")]
        public string Name { get; set; } = "";

    }

}

