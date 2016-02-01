using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace FeatureSwitcher.AwsConfiguration
{
    internal class RestClient : IRestClient
    {
        public async Task<dynamic> GetAsync(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                return await GetAsync(url, httpClient, 1);
            }
        }

        public async Task<dynamic> PutAsync(string url, object data)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                return await PutAsync(url, data, httpClient, 1);
            }
        }



        private async Task<dynamic> PutAsync(string url, object data, HttpClient httpClient, int retryCount)
        {
            string responseData = null;
            HttpStatusCode? statusCode = null;

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            try
            {
                var response = await httpClient.PutAsync(
                    url, 
                    new StringContent(serializer.Serialize(data), Encoding.UTF8, "application/json"));
                statusCode = response.StatusCode;
                responseData = await response.Content.ReadAsStringAsync();


                if (response.IsSuccessStatusCode)
                {
                    return responseData ?? serializer.Deserialize<dynamic>(responseData);
                }

                if (retryCount <= 5)
                {
                    await Task.Delay(retryCount * 150);
                    return await PutAsync(url, data, httpClient, ++retryCount);
                }

                throw new ConfigurationRequestException(response.StatusCode, responseData);
            }
            catch (Exception ex)
            {
                throw new ConfigurationRequestException(statusCode, responseData, ex);
            }
        }

        private static async Task<dynamic> GetAsync(string url, HttpClient httpClient, int retryCount)
        {
            string responseData = null;
            HttpStatusCode? statusCode = null;

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            try
            {
                var response = await httpClient.GetAsync(url);
                responseData = await response.Content.ReadAsStringAsync();
                statusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    return serializer.Deserialize<dynamic>(responseData);
                }

                if (retryCount <= 5)
                {
                    await Task.Delay(retryCount * 150);
                    return await GetAsync(url, httpClient, ++retryCount);
                }

                throw new ConfigurationRequestException(response.StatusCode, responseData);
            }
            catch (Exception ex)
            {
                throw new ConfigurationRequestException(statusCode, responseData, ex);
            }
        }
    }
}
