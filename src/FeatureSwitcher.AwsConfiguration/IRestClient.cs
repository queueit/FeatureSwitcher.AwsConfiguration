using System.Threading.Tasks;

namespace FeatureSwitcher.AwsConfiguration
{
    public interface IRestClient
    {
        Task<string> GetAsync(string url);
        Task<string> PutAsync(string url);
    }
}