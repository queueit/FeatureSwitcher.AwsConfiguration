using System.Threading.Tasks;

namespace FeatureSwitcher.AwsConfiguration
{
    public interface IRestClient
    {
        Task<dynamic> GetAsync(string url);
        Task<dynamic> PutAsync(string url, object data);
    }
}