namespace GroupCalendar.Data.Remote.Model
{
    using GroupCalendar.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public partial class UserModel
    {
        [JsonProperty("email")]
        public string Email { get; set; } = "user@example.com";

        [JsonProperty("groups")]
        public List<Guid> Groups { get; set; } = new List<Guid>();

        [JsonProperty("id")]
        public string Id { get; set; } = "0";

        [JsonProperty("name")]
        public string Name { get; set; } = "Name";

        [JsonProperty("surname")]
        public string Surname { get; set; } = "Surname";

        [JsonProperty("username")]
        public string Username { get; set; } = "user";

        [JsonProperty("age")]
        public int Age { get; set; }



        public string FullNameYou
        {
            get
            {
                var you = (Id == ApplicationState.GetValue<string>("uid")) ? " (Tú)" : "";
                var fullName = $"{Name} {Surname}{you}";
                return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(fullName.ToLower());
            }
        }
    }
}
