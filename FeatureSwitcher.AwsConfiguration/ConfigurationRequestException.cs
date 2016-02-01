using System;
using System.Net;

namespace FeatureSwitcher.AwsConfiguration
{
    public class ConfigurationRequestException : Exception
    {
        public string ResponseData { get; }
        public HttpStatusCode? Statuscode { get; }

        internal ConfigurationRequestException(HttpStatusCode? statuscode, string responseData, Exception exception)
            : base(exception?.Message ?? "Unexected exception", exception)
        {
            this.ResponseData = responseData;
            this.Statuscode = statuscode;
        }

        internal ConfigurationRequestException(HttpStatusCode statuscode, string responseData)
            : base("Service returned a non successful status code")
        {
            this.ResponseData = responseData;
            this.Statuscode = statuscode;
        }
    }
}