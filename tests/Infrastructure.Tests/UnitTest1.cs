using Moq;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PocLineAPI.Domain.Entities;
using PocLineAPI.Infrastructure.Repositories;
using PocLineAPI.Infrastructure.Configuration;

namespace PocLineAPI.Infrastructure.Tests.Repositories
{
    public class MongoDbRepositoryTests
    {
        private readonly Mock<IMongoClient> _mockMongoClient;
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<IMongoCollection<Document>> _mockCollection;
        private readonly Mock<IOptions<MongoDbSettings>> _mockOptions;
        private readonly MongoDbSettings _settings;

        public MongoDbRepositoryTests()
        {
            _mockMongoClient = new Mock<IMongoClient>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockCollection = new Mock<IMongoCollection<Document>>();
            _mockOptions = new Mock<IOptions<MongoDbSettings>>();
            
            _settings = new MongoDbSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "test_db",
                DocumentCollection = "documents",
                LineEventCollection = "line_events",
                LineMessageCollection = "line_messages"
            };

            _mockOptions.Setup(x => x.Value).Returns(_settings);
            _mockMongoClient.Setup(x => x.GetDatabase(_settings.DatabaseName, null))
                .Returns(_mockDatabase.Object);
            _mockDatabase.Setup(x => x.GetCollection<Document>(_settings.DocumentCollection, null))
                .Returns(_mockCollection.Object);
        }

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Act
            var repository = new MongoDbRepository(_mockMongoClient.Object, _mockOptions.Object);

            // Assert
            Assert.NotNull(repository);
        }

        [Fact]
        public void Constructor_WithNullMongoClient_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new MongoDbRepository(null!, _mockOptions.Object));
        }

        [Fact]
        public void Constructor_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new MongoDbRepository(_mockMongoClient.Object, null!));
        }
    }
}