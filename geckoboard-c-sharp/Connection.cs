﻿using System;
using System.Text;
using System.Net.Http;
using System.Json;

namespace Geckoboard
{
    public class Connection
    {
        private HttpClient _client;
        private static string URL = "https://api.geckoboard.com/";

        public Connection(string apiKey, HttpClient httpClient = null)
        {
            initialiseHttpClient(apiKey, httpClient);
        }

        public HttpResponseMessage Get(string path)
        {
            var response = _client.GetAsync(URL + path).Result;
            CheckResponseForErrors(response);

            return response;
        }

        public HttpResponseMessage Delete(string path)
        {
            string requestUri = URL + path;
            var response = _client.DeleteAsync(requestUri).Result;
            CheckResponseForErrors(response);

            return response;
        }

        public HttpResponseMessage Put(string path, string body)
        {
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = _client.PutAsync(URL + path, content).Result;
            CheckResponseForErrors(response);

            return response;
        }

        public HttpResponseMessage Post(string path, string body)
        {
			var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = _client.PostAsync(URL + path, content).Result;
            CheckResponseForErrors(response);

            return response;
        }

        private void CheckResponseForErrors(HttpResponseMessage response)
        {
            if ((int)response.StatusCode < 400)
                return;

            string errorMessage;
            string jsonResponse = response.Content.ReadAsStringAsync().Result;

            if (IsValidJson(jsonResponse))
            {
                var ParsedResponse = JsonValue.Parse(jsonResponse);
                errorMessage = ParsedResponse["error"]["message"];
            }
            else
            {
				errorMessage = $"\"Server responded with unexpected status code (#{response.StatusCode})\"";
            }

            throw new Exception(errorMessage);
        }

        private bool IsValidJson(string json) {
			try
			{
                var ParsedResponse = JsonValue.Parse(json);
                return true;
			}
			catch (FormatException)
			{
                return false;
			}
        }

        private void initialiseHttpClient(string apiKey, HttpClient httpClient)
        {
			_client = httpClient ?? new HttpClient();
			var byteArray = Encoding.ASCII.GetBytes(apiKey + ":");
			_client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }
    }
}
