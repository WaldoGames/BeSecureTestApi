using System.Text.Json;
using System.Net;

namespace BeSecureTestApi.Services
{
    public class GroupService
    {
        public async Task<(object, HttpStatusCode)> GetGroups(string accesstoken)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://BS-Authentik:9000/api/v3/core/groups/");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Bearer "+accesstoken);
            var response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return (null, response.StatusCode);
            };
            response.EnsureSuccessStatusCode();

            string jsonString = await response.Content.ReadAsStringAsync();
            var parsedResult = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString); // Adjust type as needed.
            if (parsedResult != null && parsedResult.ContainsKey("results"))
            {
                //var pr= parsedResult["results"].ToString(); // Return only the value for the specified key.
                return (parsedResult["results"],response.StatusCode);
            }
            return (null, HttpStatusCode.InternalServerError); // Or handle cases where the key doesn't exist.
        }
    }
}
