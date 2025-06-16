using MongoDB.Driver;
using Microsoft.Extensions.Options;
using PocLineAPI.Domain.Entities;
using PocLineAPI.Domain.Interfaces;
using PocLineAPI.Infrastructure.Configuration;

namespace PocLineAPI.Infrastructure.Repositories
{
    /// <summary>
    /// MongoDB implementation of the Line message repository.
    /// </summary>
    public class MongoDbLineMessageRepository : ILineMessageRepository
    {
        private readonly IMongoCollection<LineMessage> _lineMessagesCollection;

        public MongoDbLineMessageRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
        {
            ArgumentNullException.ThrowIfNull(mongoClient);
            ArgumentNullException.ThrowIfNull(settings);
            
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _lineMessagesCollection = database.GetCollection<LineMessage>(settings.Value.LineMessageCollection);
        }

        public async Task<bool> AddAsync(LineMessage lineMessage)
        {
            try
            {
                await _lineMessagesCollection.InsertOneAsync(lineMessage);
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
                var result = await _lineMessagesCollection.DeleteOneAsync(m => m.Id == id);
                return result.DeletedCount > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<LineMessage>> GetAllAsync()
        {
            try
            {
                return await _lineMessagesCollection.Find(_ => true).ToListAsync();
            }
            catch
            {
                return new List<LineMessage>();
            }
        }

        public async Task<IEnumerable<LineMessage>> GetByEventIdAsync(string eventId)
        {
            try
            {
                return await _lineMessagesCollection.Find(m => m.EventId == eventId).ToListAsync();
            }
            catch
            {
                return new List<LineMessage>();
            }
        }

        public async Task<LineMessage?> GetByIdAsync(string id)
        {
            try
            {
                return await _lineMessagesCollection.Find(m => m.Id == id).FirstOrDefaultAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<LineMessage>> GetByUserIdAsync(string userId)
        {
            try
            {
                return await _lineMessagesCollection.Find(m => m.UserId == userId).ToListAsync();
            }
            catch
            {
                return new List<LineMessage>();
            }
        }
    }
}