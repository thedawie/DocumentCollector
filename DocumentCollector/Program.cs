using DocumentCollector.Core.Models;
using DocumentCollector.Core.Interfaces;
using DocumentCollector.Infrastructure;
using Newtonsoft.Json;
using Serilog; 

namespace DocumentCollector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            try
            {
                var configText = await File.ReadAllTextAsync("dataSources.json");
                var configs = JsonConvert.DeserializeObject<List<DataSourceConfig>>(configText) ?? new List<DataSourceConfig>();
                var connStr = "Host=localhost;Port=5432;Username=docuser;Password=docpass;Database=documentcollector";
                IDocumentRepository repo = new DocumentRepository(connStr);
                foreach (var config in configs)
                {
                    var fetcherStrategy = new FetcherStrategy();
                    IDataSourceFetcher fetcher = fetcherStrategy.GetFetcher(config.DataSourceUrl);
                    var files = await fetcher.FetchMarkdownFilesAsync(config);
                    foreach (var (fileName, content) in files)
                    {
                        var doc = new Document
                        {
                            Source = config.DataSourceUrl,
                            Category = config.Category,
                            ShortName = config.ShortName,
                            FileName = fileName,
                            Content = content,
                            FetchedAt = DateTime.UtcNow
                        };
                        await repo.SaveDocumentAsync(doc);
                        Log.Information("Saved {FileName} from {Source}", fileName, config.DataSourceUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fatal error in DocumentCollector");
            }
            Log.CloseAndFlush();
        }
    }
}
