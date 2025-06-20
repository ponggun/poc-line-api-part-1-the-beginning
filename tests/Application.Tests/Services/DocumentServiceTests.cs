using Microsoft.Extensions.Logging;
using Moq;
using PocLineAPI.Application.Interfaces;
using PocLineAPI.Application.Services;
using PocLineAPI.Domain.Entities;
using PocLineAPI.Domain.Interfaces;

namespace PocLineAPI.Application.Tests.Services
{
    public class DocumentBusinessServiceTests
    {
        private readonly Mock<IRepository> _mockRepository;
        private readonly Mock<IEmbeddingInfraService> _mockEmbeddingInfraService;
        private readonly Mock<ILogger<DocumentBusinessService>> _mockLogger;
        private readonly DocumentBusinessService _DocumentBusinessService;

        public DocumentBusinessServiceTests()
        {
            _mockRepository = new Mock<IRepository>();
            _mockEmbeddingInfraService = new Mock<IEmbeddingInfraService>();
            _mockLogger = new Mock<ILogger<DocumentBusinessService>>();
            _DocumentBusinessService = new DocumentBusinessService(_mockRepository.Object, _mockEmbeddingInfraService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetDocumentByIdAsync_CallsRepositoryGetByIdAsync()
        {
            // Arrange
            string testId = "test-id";
            var expectedDocument = new Document(testId, "Test content");
            
            _mockRepository.Setup(repo => repo.GetByIdAsync(testId))
                .ReturnsAsync(expectedDocument);

            // Act
            var result = await _DocumentBusinessService.GetDocumentByIdAsync(testId);

            // Assert
            Assert.Equal(expectedDocument, result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(testId), Times.Once);
        }

        [Fact]
        public async Task GetAllDocumentsAsync_CallsRepositoryGetAllAsync()
        {
            // Arrange
            var expectedDocuments = new List<Document> 
            {
                new Document("1", "Content 1"),
                new Document("2", "Content 2")
            };
            
            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedDocuments);

            // Act
            var result = await _DocumentBusinessService.GetAllDocumentsAsync();

            // Assert
            Assert.Equal(expectedDocuments, result);
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateDocumentAsync_CallsRepositoryAddAsync()
        {
            // Arrange
            var document = new Document("test-id", "Test content");
            
            _mockRepository.Setup(repo => repo.AddAsync(document))
                .ReturnsAsync(true);

            // Act
            var result = await _DocumentBusinessService.CreateDocumentAsync(document);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.AddAsync(document), Times.Once);
        }

        [Fact]
        public async Task UpdateDocumentAsync_CallsRepositoryUpdateAsync()
        {
            // Arrange
            var document = new Document("test-id", "Updated content");
            
            _mockRepository.Setup(repo => repo.UpdateAsync(document))
                .ReturnsAsync(true);

            // Act
            var result = await _DocumentBusinessService.UpdateDocumentAsync(document);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.UpdateAsync(document), Times.Once);
        }

        [Fact]
        public async Task DeleteDocumentAsync_CallsRepositoryDeleteAsync()
        {
            // Arrange
            string testId = "test-id";
            
            _mockRepository.Setup(repo => repo.DeleteAsync(testId))
                .ReturnsAsync(true);

            // Act
            var result = await _DocumentBusinessService.DeleteDocumentAsync(testId);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.DeleteAsync(testId), Times.Once);
        }

        [Fact]
        public async Task SearchSimilarDocumentsAsync_ThrowsNotImplementedException()
        {
            // Arrange
            string query = "test query";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotImplementedException>(() => 
                _DocumentBusinessService.SearchSimilarDocumentsAsync(query));
            
            Assert.Equal("Semantic search not yet implemented", exception.Message);
        }
    }
}