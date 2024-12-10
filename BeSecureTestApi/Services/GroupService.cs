using System.Text.Json;

namespace BeSecureTestApi.Services
{
    public class GroupService
    {
        public async Task<object> GetGroups()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:9000/api/v3/core/groups/");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Bearer tOfsWazm88aVjeEAQMUwzTh6xzfnnGjWM0zUND6sXt0qsyTwQ1vTwYsorTRu");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string jsonString = await response.Content.ReadAsStringAsync();
            var parsedResult = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString); // Adjust type as needed.
            if (parsedResult != null && parsedResult.ContainsKey("results"))
            {
                //var pr= parsedResult["results"].ToString(); // Return only the value for the specified key.
                return parsedResult["results"];
            }
            return null; // Or handle cases where the key doesn't exist.
        }
    }
}
