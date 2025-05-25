using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using DocumentCollector.Core.Models;
using Npgsql;

namespace DocumentCollector.Infrastructure
{
    public class DocumentRepository : Core.Interfaces.IDocumentRepository
    {
        private readonly string _connectionString;
        public DocumentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task SaveDocumentAsync(Document doc)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(
                "INSERT INTO Documents (source, category, shortName, fileName, content, fetchedAt) VALUES (@Source, @Category, @ShortName, @FileName, @Content, @FetchedAt)",
                doc);
        }
    }
}
