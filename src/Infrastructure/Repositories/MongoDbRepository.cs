using MongoDB.Driver;
using Microsoft.Extensions.Options;
using PocLineAPI.Domain.Entities;
using PocLineAPI.Domain.Interfaces;
using PocLineAPI.Infrastructure.Configuration;

namespace PocLineAPI.Infrastructure.Repositories
{
    /// <summary>
    /// MongoDB implementation of the document repository.
    /// </summary>
    public class MongoDbRepository : IRepository
    {
        private readonly IMongoCollection<Document> _documentsCollection;

        public MongoDbRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
        {
            ArgumentNullException.ThrowIfNull(mongoClient);
            ArgumentNullException.ThrowIfNull(settings);
            
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _documentsCollection = database.GetCollection<Document>(settings.Value.DocumentCollection);
        }

        public async Task<bool> AddAsync(Document document)
        {
            try
            {
                await _documentsCollection.InsertOneAsync(document);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var result = await _documentsCollection.DeleteOneAsync(d => d.Id == id);
                return result.DeletedCount > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            try
            {
                return await _documentsCollection.Find(_ => true).ToListAsync();
            }
            catch
            {
                return new List<Document>();
            }
        }

        public async Task<Document> GetByIdAsync(string id)
        {
            try
            {
                var document = await _documentsCollection.Find(d => d.Id == id).FirstOrDefaultAsync();
                return document ?? new Document(id, "Document not found");
            }
            catch
            {
                return new Document(id, "Error retrieving document");
            }
        }

        public async Task<bool> UpdateAsync(Document document)
        {
            try
            {
                var result = await _documentsCollection.ReplaceOneAsync(d => d.Id == document.Id, document);
                return result.ModifiedCount > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}