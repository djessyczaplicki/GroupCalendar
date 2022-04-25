using GroupCalendar.Core;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Proyecto_Intermodular.api
{
    public class ApiClient
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private static string GetToken() => ApplicationState.GetValue<string>("token");

        public static async Task<string> Get(string uri)
        {
            uri += "?auth=" + GetToken();

            HttpResponseMessage response = await httpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                throw new ApiException(await response.Content.ReadAsStringAsync());
        }


        public static async Task<string> Post(string uri, object data)
        {
            uri += "?auth=" + GetToken();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(data, GetJsonSettings()));
            stringContent.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = await httpClient.PostAsync(uri, stringContent);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                throw new ApiException(await response.Content.ReadAsStringAsync());
        }


        public static async Task<string> Patch(string uri, object data)
        {
            uri += "?auth=" + GetToken();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(data, GetJsonSettings()));
            stringContent.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = await httpClient.PatchAsync(uri, stringContent);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new ItemNotFoundException();
                throw new ApiException(await response.Content.ReadAsStringAsync());
            }
        }

        public static async Task<string> Put(string uri, object data)
        {
            uri += "?auth=" + GetToken();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(data, GetJsonSettings()));
            stringContent.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = await httpClient.PutAsync(uri, stringContent);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new ItemNotFoundException();
                throw new ApiException(await response.Content.ReadAsStringAsync());
            }
        }


        public static async Task<string> Delete(string uri)
        {
            uri += "?auth=" + GetToken();

            HttpResponseMessage response = await httpClient.DeleteAsync(uri);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new ItemNotFoundException(await response.Content.ReadAsStringAsync());
                throw new ApiException(await response.Content.ReadAsStringAsync());
            }
        }


        internal static JsonSerializerSettings GetJsonSettings()
        {
            return new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ" };
        }
    }
}
