using DocumentCollector.Core.Interfaces;

namespace DocumentCollector.Infrastructure
{
    public class FetcherStrategy
    {
        public IDataSourceFetcher GetFetcher(string dataSourceUrl)
        {
            if (dataSourceUrl.Contains("github.com"))
                return new GitHubFetcher();
            if (dataSourceUrl.Contains("dev.azure.com"))
                return new AzureDevOpsFetcher();
            throw new System.NotSupportedException($"Unknown data source: {dataSourceUrl}");
        }
    }
}
