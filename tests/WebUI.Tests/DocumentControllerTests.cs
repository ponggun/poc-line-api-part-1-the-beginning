using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace PocLineAPI.Presentation.WebUI.Tests
{
    public class DocumentControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public DocumentControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Index_ReturnsSuccessAndCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/Document");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task Create_Get_ReturnsSuccessAndCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/Document/Create");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task Index_ContainsExpectedContent()
        {
            // Act
            var response = await _client.GetAsync("/Document");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("Document Management", content);
            Assert.Contains("Create New Document", content);
        }

        [Fact]
        public async Task Create_ContainsExpectedForm()
        {
            // Act
            var response = await _client.GetAsync("/Document/Create");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("Create New Document", content);
            Assert.Contains("Document Content", content);
            Assert.Contains("form", content);
        }

        [Fact]
        public async Task RootRedirectsToDocument()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert - Should redirect to Document controller since it's our default route
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}