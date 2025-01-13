using BeSecureTestApi.Dto;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net;

namespace BeSecureTestApi.Services
{
    public class UserServices
    {
        private static readonly HttpClient client = new HttpClient();
        //BS-Authentik
        public async Task<(object, System.Net.HttpStatusCode)> GetUsers(string accessToken)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://BS-Authentik:9000/api/v3/core/users/");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Bearer "+accessToken);

            var response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized||response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return (null, response.StatusCode);
            };
            response.EnsureSuccessStatusCode();

            string jsonString = await response.Content.ReadAsStringAsync();
            var parsedResult = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString); // Adjust type as needed.
            if (parsedResult != null && parsedResult.ContainsKey("results"))
            {
                //var pr= parsedResult["results"].ToString(); // Return only the value for the specified key.
                return (parsedResult["results"], response.StatusCode);
            }
            return (null, System.Net.HttpStatusCode.BadRequest); // Or handle cases where the key doesn't exist.
        }

        public async Task<(UsersDto?, HttpStatusCode)> getUserByName (string UserName, string accessToken)
        {
            (object, HttpStatusCode) users = await GetUsers(accessToken);
           
            if (users.Item2 != HttpStatusCode.OK)
            {
                return (null, users.Item2);
            }

            var jsonArray = JsonSerializer.Deserialize<List<UsersDto>>(users.Item1.ToString()); // Replace `object` with a specific type if known

            UsersDto? user= jsonArray.Where(j => j.username == UserName).FirstOrDefault();

            return (user, HttpStatusCode.OK);
        }
       
        public async Task<(bool, HttpStatusCode)> CreateUserAsync(NewUserDto newUser)
        {
            const string url = "http://BS-Authentik:9000/api/v3/core/users/"; // Replace with your API URL

            try
            {
                NewUserPlusType newUserPlus = new NewUserPlusType();
                newUserPlus.groups = new string[]{newUser.groups};
                newUserPlus.username = newUser.username; 
                newUserPlus.password = newUser.password;
                newUserPlus.email = newUser.email;
                newUserPlus.name = newUser.name;
                newUserPlus.type = "internal";
                //newUserPlus.accessToken = newUser.accessToken;
                // Serialize the DTO to JSON
                var jsonContent = JsonSerializer.Serialize(newUserPlus);

                // Create the request content
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Add authorization header
                client.DefaultRequestHeaders.Clear(); // Clear any existing headers
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer "+newUser.accessToken);

                // Send the POST request
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return (false, response.StatusCode);
                };
                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                return (true, HttpStatusCode.OK);

                // Read and return the response content as a string
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                return (false, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<HttpStatusCode> CreatePassword(string newUserPassword, string username, string accessToken)
        {
            //const string url = "http://BS-Authentik:9000/api/v3/core/users/"; // Replace with your API URL

            try
            {
                (UsersDto, HttpStatusCode) user = await getUserByName(username, accessToken);
                if (user.Item2 == HttpStatusCode.Forbidden || user.Item2 == HttpStatusCode.Unauthorized)
                {
                    return HttpStatusCode.Unauthorized;
                }
                if (user.Item2 != HttpStatusCode.OK)
                {
                    return (user.Item2);
                }

                // Serialize the DTO to JSON
                var jsonContent = JsonSerializer.Serialize(new { password = newUserPassword });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Add authorization header
                client.DefaultRequestHeaders.Clear(); // Clear any existing headers
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer "+accessToken);

                // Send the POST request
                HttpResponseMessage response = await client.PostAsync("http://BS-Authentik:9000/api/v3/core/users/" + user.Item1.pk+ "/set_password/", content);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<HttpStatusCode> ChangeUserActiveStatus(UserActiveDto userdto)
        {
            try
            {
                (UsersDto, HttpStatusCode) user = await getUserByName(userdto.username, userdto.accessToken);
                if (user.Item2 == HttpStatusCode.Forbidden || user.Item2 == HttpStatusCode.Unauthorized)
                {
                    return HttpStatusCode.Unauthorized;
                }
                if (user.Item1==null)
                {
                    return HttpStatusCode.InternalServerError;
                }
                // Serialize the DTO to JSON
                var jsonContent = JsonSerializer.Serialize(new { is_active = userdto.is_active });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Add authorization header
                client.DefaultRequestHeaders.Clear(); // Clear any existing headers
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer "+userdto.accessToken);

                // Send the PATCH request
                HttpResponseMessage response = await client.PatchAsync("http://BS-Authentik:9000/api/v3/core/users/" + user.Item1.pk + "/", content);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                return HttpStatusCode.InternalServerError;
            }
        }
    }

    public class NewUserPlusType
    {
        public string username { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string[] groups { get; set; }
        public string password { get; set; }
        public string type { get; set; }
    }
}
