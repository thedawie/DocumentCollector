using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using DocumentCollector.Core.Models;
using Serilog;

namespace DocumentCollector.Infrastructure
{
    public class AzureDevOpsFetcher : Core.Interfaces.IDataSourceFetcher
    {
        public async Task<IEnumerable<(string fileName, string content)>> FetchMarkdownFilesAsync(DataSourceConfig config)
        {
            var results = new List<(string, string)>();
            try
            {
                // Example: https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repoId}/items?scopePath=/docs/specs&recursionLevel=Full&api-version=7.0
                // User must provide a full API URL in dataSourceUrl for Azure DevOps
                var apiUrl = config.DataSourceUrl + "/_apis/git/repositories/" + config.ShortName + "/items?scopePath=/" + config.FolderName + "&recursionLevel=Full&api-version=7.0";
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{config.Token}")));
                var resp = await client.GetAsync(apiUrl);
                if (!resp.IsSuccessStatusCode)
                {
                    Log.Error("Azure DevOps fetch failed: {Status} {Reason}", resp.StatusCode, resp.ReasonPhrase);
                    return results;
                }
                var json = await resp.Content.ReadAsStringAsync();
                var files = JsonDocument.Parse(json).RootElement.GetProperty("value");
                foreach (var file in files.EnumerateArray())
                {
                    if (file.GetProperty("path").GetString().EndsWith(".md"))
                    {
                        var contentUrl = file.GetProperty("url").GetString();
                        var mdResp = await client.GetAsync(contentUrl);
                        if (mdResp.IsSuccessStatusCode)
                        {
                            var content = await mdResp.Content.ReadAsStringAsync();
                            results.Add((file.GetProperty("path").GetString(), content));
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Error fetching from Azure DevOps");
            }
            return results;
        }
    }
}
