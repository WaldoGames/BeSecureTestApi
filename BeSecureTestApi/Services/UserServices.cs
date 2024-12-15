using BeSecureTestApi.Dto;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BeSecureTestApi.Services
{
    public class UserServices
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<object> GetUsers()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:9000/api/v3/core/users/");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Bearer p1dyjpMLQIcOCEBh4tlKOWo9chsKdBsFW72eWbQ99fayIHC5um1N8rqmUePS");

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

        public async Task<UsersDto?> getUserByName (string UserName)
        {
            object users = await GetUsers();

            var jsonArray = JsonSerializer.Deserialize<List<UsersDto>>(users.ToString()); // Replace `object` with a specific type if known

            UsersDto? user= jsonArray.Where(j => j.username == UserName).FirstOrDefault();

            return user;
        }
        public async Task<bool> CreateUserAsync(NewUserDto newUser)
        {
            const string url = "http://localhost:9000/api/v3/core/users/"; // Replace with your API URL

            try
            {
                // Serialize the DTO to JSON
                var jsonContent = JsonSerializer.Serialize(newUser);

                // Create the request content
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Add authorization header
                client.DefaultRequestHeaders.Clear(); // Clear any existing headers
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer p1dyjpMLQIcOCEBh4tlKOWo9chsKdBsFW72eWbQ99fayIHC5um1N8rqmUePS");

                // Send the POST request
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                return true;

                // Read and return the response content as a string
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public async Task<string> CreatePassword(string newUserPassword, string username)
        {
            //const string url = "http://localhost:9000/api/v3/core/users/"; // Replace with your API URL

            try
            {
                UsersDto user = await getUserByName(username);


                // Serialize the DTO to JSON
                var jsonContent = JsonSerializer.Serialize(new { password = newUserPassword });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Add authorization header
                client.DefaultRequestHeaders.Clear(); // Clear any existing headers
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer p1dyjpMLQIcOCEBh4tlKOWo9chsKdBsFW72eWbQ99fayIHC5um1N8rqmUePS");

                // Send the POST request
                HttpResponseMessage response = await client.PostAsync("http://localhost:9000/api/v3/core/users/"+user.pk+ "/set_password/", content);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Read and return the response content as a string
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<string> ChangeUserActiveStatus(UserActiveDto userdto)
        {
            try
            {
                UsersDto user = await getUserByName(userdto.username);
                if (user==null)
                {
                    return null;
                }
                // Serialize the DTO to JSON
                var jsonContent = JsonSerializer.Serialize(new { is_active = userdto.is_active });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Add authorization header
                client.DefaultRequestHeaders.Clear(); // Clear any existing headers
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer p1dyjpMLQIcOCEBh4tlKOWo9chsKdBsFW72eWbQ99fayIHC5um1N8rqmUePS");

                // Send the PATCH request
                HttpResponseMessage response = await client.PatchAsync("http://localhost:9000/api/v3/core/users/" + user.pk + "/", content);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Read and return the response content as a string
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}
