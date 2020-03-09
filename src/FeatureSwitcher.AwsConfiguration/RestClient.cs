using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FeatureSwitcher.AwsConfiguration
{
    internal class RestClient : IRestClient
    {
        public async Task<dynamic> GetAsync(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                return await GetAsync(url, httpClient, 1).ConfigureAwait(false);
            }
        }

        public async Task<string> PutAsync(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                return await PutAsync(url, httpClient, 1).ConfigureAwait(false);
            }
        }



        private async Task<string> PutAsync(string url, HttpClient httpClient, int retryCount)
        {
            string responseData = null;
            HttpStatusCode? statusCode = null;

            try
            {
                var response = await httpClient
                    .PutAsync(
                        url, 
                        new StringContent("", Encoding.UTF8, "application/json"))
                    .ConfigureAwait(false);
                statusCode = response.StatusCode;
                responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return responseData;
                }

                if (retryCount <= 5)
                {
                    await Task.Delay(retryCount * 150).ConfigureAwait(false);
                    return await PutAsync(url, httpClient, ++retryCount).ConfigureAwait(false);
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

            try
            {
                var response = await httpClient.GetAsync(url).ConfigureAwait(false);
                responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                statusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    return JObject.Parse(responseData);
                }

                if (retryCount <= 5)
                {
                    await Task.Delay(retryCount * 150).ConfigureAwait(false);
                    return await GetAsync(url, httpClient, ++retryCount).ConfigureAwait(false);
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
