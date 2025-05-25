using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DocumentCollector.Core.Models;
using Serilog;

namespace DocumentCollector.Infrastructure
{
    public class GitHubFetcher : Core.Interfaces.IDataSourceFetcher
    {
        public async Task<IEnumerable<(string fileName, string content)>> FetchMarkdownFilesAsync(DataSourceConfig config)
        {
            var results = new List<(string, string)>();
            try
            {
                var apiUrl = $"{config.DataSourceUrl}/contents/{config.FolderName}?ref=main";

                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DocumentCollector", "1.0"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", config.Token);

                var resp = await client.GetAsync(apiUrl);
                if (!resp.IsSuccessStatusCode)
                {
                    Log.Error("GitHub fetch failed: {Status} {Reason}", resp.StatusCode, resp.ReasonPhrase);
                    return results;
                }
                
                var json = await resp.Content.ReadAsStringAsync();
                var files = JsonDocument.Parse(json).RootElement;
                foreach (var file in files.EnumerateArray())
                {
                    if (file.GetProperty("name").GetString().EndsWith(".md"))
                    {
                        var downloadUrl = file.GetProperty("download_url").GetString();
                        var mdResp = await client.GetAsync(downloadUrl);
                        if (mdResp.IsSuccessStatusCode)
                        {
                            var content = await mdResp.Content.ReadAsStringAsync();
                            results.Add((file.GetProperty("name").GetString(), content));
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Error fetching from GitHub");
            }
            return results;
        }
    }
}
