using MongoDB.Driver;
using Microsoft.Extensions.Options;
using PocLineAPI.Domain.Entities;
using PocLineAPI.Domain.Interfaces;
using PocLineAPI.Infrastructure.Configuration;

namespace PocLineAPI.Infrastructure.Repositories
{
    /// <summary>
    /// MongoDB implementation of the Line event repository.
    /// </summary>
    public class MongoDbLineEventRepository : ILineEventRepository
    {
        private readonly IMongoCollection<LineEvent> _lineEventsCollection;

        public MongoDbLineEventRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
        {
            ArgumentNullException.ThrowIfNull(mongoClient);
            ArgumentNullException.ThrowIfNull(settings);
            
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _lineEventsCollection = database.GetCollection<LineEvent>(settings.Value.LineEventCollection);
        }

        public async Task<bool> AddAsync(LineEvent lineEvent)
        {
            try
            {
                await _lineEventsCollection.InsertOneAsync(lineEvent);
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
                var result = await _lineEventsCollection.DeleteOneAsync(e => e.Id == id);
                return result.DeletedCount > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<LineEvent>> GetAllAsync()
        {
            try
            {
                return await _lineEventsCollection.Find(_ => true).ToListAsync();
            }
            catch
            {
                return new List<LineEvent>();
            }
        }

        public async Task<LineEvent?> GetByIdAsync(string id)
        {
            try
            {
                return await _lineEventsCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<LineEvent>> GetByTypeAsync(string type)
        {
            try
            {
                return await _lineEventsCollection.Find(e => e.Type == type).ToListAsync();
            }
            catch
            {
                return new List<LineEvent>();
            }
        }
    }
}