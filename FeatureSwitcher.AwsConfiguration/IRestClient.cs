﻿using System.Threading.Tasks;

namespace FeatureSwitcher.AwsConfiguration
{
    public interface IRestClient
    {
        Task<dynamic> GetAsync(string url);
    }
}