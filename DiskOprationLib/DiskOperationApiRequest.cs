using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiskOprationLib
{
    public static class DiskOperationApiRequest
    {
        private static string BaseURL = "https://llg-api.dev.area51labs.it/api/v1/";
        public static async Task<dynamic> GetDiskOperationApi(string EndPoint)
        {
            var httpClient = new HttpClient();

            // Set the base URL for the API endpoint
            string baseUrl = BaseURL;

            // The endpoint you want to request
            string endpoint = EndPoint;

            try
            {
                // Make the GET request to the API
                HttpResponseMessage response = await httpClient.GetAsync($"{baseUrl}{endpoint}");

                // Check if the request was successful (status code 200-299)
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Process the JSON response or do something else with it
                    dynamic list = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    return list;
                }
                else
                {
                    // Request failed, handle the error
                    Console.WriteLine($"Request failed with status code {response.StatusCode}: {response.ReasonPhrase}");
                    return "";
                }

            }
            catch (HttpRequestException ex)
            {
                // Handle any network-related errors
                throw ex;
            }
        }


        public static async Task<dynamic> PostDiskOperationApi(dynamic parameter, string EndPoint)
        {
            var httpClient = new HttpClient();

            // Set the base URL for the API endpoint
            string baseUrl = BaseURL;

            // The endpoint you want to request
            string endpoint = EndPoint;

            try
            {
                var data = parameter;
                var jsonConvertData = JsonConvert.SerializeObject(data);
                // Convert the data to a JSON string
                var jsonData = new StringContent(
                    jsonConvertData,
                    Encoding.UTF8,
                    "application/json"
                );
                // Make the GET request to the API
                HttpResponseMessage response = await httpClient.PostAsync($"{baseUrl}{endpoint}", jsonData);

                // Check if the request was successful (status code 200-299)
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Process the JSON response or do something else with it
                    dynamic list = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    return list;
                }
                else
                {
                    // Request failed, handle the error
                    Console.WriteLine($"Request failed with status code {response.StatusCode}: {response.ReasonPhrase}");
                    return "";
                }

            }
            catch (HttpRequestException ex)
            {
                // Handle any network-related errors
                throw ex;
            }
        }
    }
}
