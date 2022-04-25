using GroupCalendar.Core;
using GroupCalendar.Data.Remote.Model;
using Newtonsoft.Json;
using Proyecto_Intermodular.api;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroupCalendar.Data.Network
{
    internal class Repository
    {
        string baseUrl = "https://groupcalendar-53829-default-rtdb.europe-west1.firebasedatabase.app";

        private void SetupRequest(RestRequest request, object? obj = null)
        {
            var token = ApplicationState.GetValue<string>("token");
            if (token == null)
            {
                throw new Exception("Empty token");
            }
            request.AddParameter("auth", token);
            //request.AddHeader("Accept", "application/json");
            //request.AddHeader("Content-Type", "application/json");
            if (obj != null)
            {
                request.RequestFormat = DataFormat.Json;
                var settings = new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ" };
                string json = JsonConvert.SerializeObject(obj, settings);
                request.AddStringBody(json, DataFormat.Json);

            }
        }

        public async Task<GroupModel> GetGroupByIdAsync(string id)
        {
            var client = new RestClient();
            var request = new RestRequest($"{baseUrl}/groups/{id}.json", Method.Get);
            SetupRequest(request);

            var response = await client.GetAsync(request);
            var data = JsonConvert.DeserializeObject<GroupModel>(response.Content);
            return data;
        }

        public async Task<List<EventModel>> UpdateGroupEventsAsync(GroupModel group)
        {
            try
            {
                var uri = $"{baseUrl}/groups/{group.Id}/events.json";
                var updatedGroupEventsJson = await ApiClient.Put(uri, group.Events);
                var updatedGroupEvents = JsonConvert.DeserializeObject<List<EventModel>>(updatedGroupEventsJson, ApiClient.GetJsonSettings());
                return updatedGroupEvents;
            }
            catch (ApiException ex)
            {
                throw new ApiException(ex.Message);
            }

        }
    }

}