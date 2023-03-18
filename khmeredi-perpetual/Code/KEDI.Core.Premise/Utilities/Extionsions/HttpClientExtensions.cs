using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KEDI.Core.Net.Http.Extionsions
{
    public static class HttpClientExtensions
    {
        private static string Serialize<T>(T objectBody)
        {
            return JsonConvert.SerializeObject(objectBody, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        private static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static async Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient httpClient, string url, string jsonBody)
        {
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content);
            return response;
        }

        public static async Task<HttpResponseMessage> PostAsFormAsync<T>(this HttpClient httpClient, string url, T objectBody)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs = objectBody.GetType().GetProperties()
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(objectBody, null).ToString());
            var content = new FormUrlEncodedContent(keyValuePairs);
            var response = await httpClient.PostAsync(url, content);
            return response;
        }

        public static async Task<HttpResponseMessage> PostAsFormDataAsync<T>(this HttpClient httpClient, string url, T objectBody)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs = objectBody.GetType().GetProperties()
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(objectBody, null).ToString());
            var content = new MultipartFormDataContent();
            foreach (var kv in keyValuePairs)
            {
                content.Add(new StringContent(kv.Value, Encoding.UTF8), kv.Key);
            }
            var response = await httpClient.PostAsync(url, content);
            return response;
        }

        public static async Task<HttpResponseMessage> PostAsStringAsync<T>(this HttpClient httpClient, string url, T jsonBody)
        {
            Type inputType = jsonBody.GetType();
            if (inputType.IsPrimitive)
            {
                return await httpClient.PostAsJsonAsync(url, jsonBody);
            }

            if (inputType == typeof(string))
            {
                return await httpClient.PostAsJsonAsync(url, Serialize(jsonBody));       
            }

            string jsonContent = Serialize(jsonBody);
            string jsonText = Serialize(jsonContent);
            return await httpClient.PostAsJsonAsync(url, jsonText);
        }
    }
}
