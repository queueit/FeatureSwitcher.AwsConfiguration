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

        private static async Task<dynamic> GetAsync(string url, HttpClient httpClient, int retryCount)
        {
            string data = null;
            HttpStatusCode? statusCode = null;
            try
            {
                var response = await httpClient.GetAsync(url);
                data = await response.Content.ReadAsStringAsync();
                statusCode = response.StatusCode;

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var dynamic = serializer.Deserialize<dynamic>(data);

                if (response.IsSuccessStatusCode)
                {
                    return dynamic;
                }

                if (retryCount <= 5)
                {
                    await Task.Delay(retryCount * 150);
                    return await GetAsync(url, httpClient, ++retryCount);
                }

                throw new ConfigurationRequestException(response.StatusCode, dynamic.ToString());
            }
            catch (Exception ex)
            {
                throw new ConfigurationRequestException(statusCode, data, ex);
            }
        }
    }
}
