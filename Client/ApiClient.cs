using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ApiClient
    {
        private readonly HttpClient _client;

        public ApiClient(string apiUrl)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(apiUrl);
        }

        public async Task<string> PostDataAsync(string endpoint, object data)
        {
            try
            {
                // Serialize your data object to JSON
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Make the POST request
                var response = await _client.PostAsync(endpoint, content);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Read the response as a string
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}
