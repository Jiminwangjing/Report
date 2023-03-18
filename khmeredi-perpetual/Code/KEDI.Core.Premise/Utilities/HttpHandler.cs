using KEDI.Core.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KEDI.Core.Http
{
    public class HttpHandler
    {
        public HttpHandler()
        {
            _handler = new HttpClientHandler();
            _handler.ServerCertificateCustomValidationCallback = delegate { return true; };
            _httpClient = new HttpClient(_handler);          
        }
        
        public HttpHandler(HttpClientHandler clientHandler)
        {
            _handler = clientHandler;
            _httpClient = new HttpClient(_handler);
        }
        
        private readonly HttpClientHandler _handler;
        private HttpClient _httpClient;
        public string Referrer
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _httpClient.DefaultRequestHeaders.Referrer = new Uri(value);
                }
            }
        }

        public string BaseAddress
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _httpClient.BaseAddress = new Uri(value);
                }
            }
        }
        public string Authorization
        {
            set
            {
                if (AuthenticationHeaderValue.TryParse(value, out AuthenticationHeaderValue _auth))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = _auth;
                }
            }
        }

        public HttpClient HttpClient 
        {
            set
            {
                _httpClient = value;
            }
            get
            {
                return _httpClient;
            } 
        }
       
        public IDictionary<string, string> Headers
        {
            set
            {
                foreach (var header in value)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
        }

        public void AddHeader(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _httpClient.DefaultRequestHeaders.Add(key, value);
            }
        }

        public void AddHeaderRange(IDictionary<string, string> headers)
        {
            foreach(var header in headers)
            {
                AddHeader(header.Key, header.Value);
            }
        }

        public void AcceptHeader(MediaTypeWithQualityHeaderValue mediaHeader)
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(mediaHeader);
        }

        public void TryAcceptHeader(string mediaTypeHeader)
        {
            _httpClient.DefaultRequestHeaders.Accept.TryParseAdd(mediaTypeHeader);
        }

        public Task<HttpResponseMessage> PostContentAsync(string endpoint, HttpContent content,
        Action<Task<HttpResponseMessage>> action = null)
        {           
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = _httpClient.PostAsync(endpoint, content).Result;
            }
            catch (Exception ex) { response.ReasonPhrase = ex.Message; }

            action?.Invoke(Task.FromResult(response));
            return Task.FromResult(response);
        }

        //In json (application/json)
        public Task<HttpResponseMessage> PostJsonAsync(string url, string jsonBody, Action<Task<HttpResponseMessage>> action = null)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            return PostContentAsync(url, content, action);
        }

        public Task<HttpResponseMessage> PostAsync<T>(string url, T objectBody, Action<Task<HttpResponseMessage>> action = null)
        {
            if (objectBody.GetType().IsPrimitive || objectBody.GetType() == typeof(string))
            {
                if(objectBody.GetType() == typeof(string))
                {
                    if (IsValidJson(objectBody.ToString()))
                    {
                        return PostJsonAsync(url, objectBody.ToString(), action);
                    }
                    string textContent = string.Format("\"{0}\"", objectBody);
                    return PostJsonAsync(url, textContent, action);
                }
                return PostJsonAsync(url, objectBody.ToString(), action);
            }
            else
            {
                string jsonContent = JsonConvert.SerializeObject(objectBody);
                return PostJsonAsync(url, jsonContent, action);
            }
        }

        public Task<HttpResponseMessage> PostFormAsync<T>(string url, T objectBody,
            Action<Task<HttpResponseMessage>> action = null)
        {            
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs = objectBody.GetType().GetProperties()
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(objectBody, null).ToString());
            var content = new FormUrlEncodedContent(keyValuePairs);
            return PostContentAsync(url, content, action);
        }

        public Task<HttpResponseMessage> PostFormDataAsync<T>(string url, T objectBody,
            Action<Task<HttpResponseMessage>> action = null)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs = objectBody.GetType().GetProperties()
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(objectBody, null).ToString());
            var content = new MultipartFormDataContent();
            foreach (var kv in keyValuePairs)
            {
                content.Add(new StringContent(kv.Value, Encoding.UTF8), kv.Key);
            }

            return PostContentAsync(url, content, action);
        }

        public Task<HttpResponseMessage> PostStringAsync<T>(string url, T jsonBody, Action<Task<HttpResponseMessage>> action = null)
        {
            var jsonContent = JsonConvert.SerializeObject(jsonBody);
            if (jsonBody.GetType() == typeof(string))
            {
                if (IsValidJson(jsonBody.ToString()))
                {                  
                    return PostJsonAsync(url, jsonContent, action);
                }
                string text = string.Format("\"{0}\"", jsonBody);
                return PostJsonAsync(url, text, action);
            }
            else
            {
                string jsonText = JsonConvert.SerializeObject(jsonContent);
                return PostJsonAsync(url, jsonText, action);
            }
        }

        public Task<HttpResponseMessage> GetAsync(string endpoint, Action<Task<HttpResponseMessage>> action = null)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = _httpClient.GetAsync(endpoint).Result;
            } catch(Exception ex) { response.ReasonPhrase = ex.Message; }

            action?.Invoke(Task.FromResult(response));
            return Task.FromResult(response);
        }

        public Task<string> GetStringAsync(string url, Action<string> action = null)
        {
            return GetAsync(url).Result.Content?.ReadAsStringAsync();
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
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}



