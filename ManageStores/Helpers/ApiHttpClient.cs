using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace ManageStores.Helpers
{
    public class ApiHttpClient
    {
        public static HttpClient GetApiClient()
        {
            var authorizationDetails = Convert.ToBase64String(Encoding.Default.GetBytes(ConfigurationManager.AppSettings["ApiUserName"] +
            ":" + ConfigurationManager.AppSettings["ApiPassword"]));
            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
            return GetClient(authorizationDetails, apiUrl);
        }

        private static HttpClient GetClient(string AuthorizationDetails, string apiUrl)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", $"Basic {AuthorizationDetails}");

            var baseAddress = apiUrl;
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}