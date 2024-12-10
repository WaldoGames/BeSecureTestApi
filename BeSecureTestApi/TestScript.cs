using System.Text;

namespace BeSecureTestApi
{
    public class TestScript
    {
        public async Task<string> test()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:9000/api/v3/core/groups/");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Bearer tOfsWazm88aVjeEAQMUwzTh6xzfnnGjWM0zUND6sXt0qsyTwQ1vTwYsorTRu");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string s=await response.Content.ReadAsStringAsync();
            return s;
        }
    }
}
