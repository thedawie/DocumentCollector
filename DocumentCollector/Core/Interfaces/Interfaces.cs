using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentCollector.Core.Models;

namespace DocumentCollector.Core.Interfaces
{
    public interface IDataSourceFetcher
    {
        Task<IEnumerable<(string fileName, string content)>> FetchMarkdownFilesAsync(DataSourceConfig config);
    }

    public interface IDocumentRepository
    {
        Task SaveDocumentAsync(Document doc);
    }
}
