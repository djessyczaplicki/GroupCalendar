namespace GroupCalendar.Data.Remote.Model
{
    using Newtonsoft.Json;
    using System;

    public partial class UserModel
    {
        [JsonProperty("email")]
        public string Email { get; set; } = "user@example.com";

        [JsonProperty("groups")]
        public Guid[] Groups { get; set; } = new Guid[] { };

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
    }
}
