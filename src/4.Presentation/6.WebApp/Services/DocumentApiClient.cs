using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using PocLineAPI.Domain.Entities;

namespace PocLineAPI.Presentation.WebApp.Services
{
    public class DocumentApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        
        public DocumentApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            httpClient.BaseAddress = new Uri(configuration["ApiBaseUrl"] ?? "https://localhost:5001/");
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
        {
            var response = await _httpClient.GetAsync("api/document");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<IEnumerable<Document>>(_jsonOptions) 
                ?? Enumerable.Empty<Document>();
        }

        public async Task<Document?> GetDocumentByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"api/document/{id}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
                
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Document>(_jsonOptions);
        }

        public async Task<bool> CreateDocumentAsync(Document document)
        {
            var response = await _httpClient.PostAsJsonAsync("api/document", document);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateDocumentAsync(Document document)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/document/{document.Id}", document);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDocumentAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/document/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<Document>> SearchSimilarDocumentsAsync(string query, int limit = 5)
        {
            var response = await _httpClient.GetAsync($"api/document/search?query={Uri.EscapeDataString(query)}&limit={limit}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<IEnumerable<Document>>(_jsonOptions) 
                ?? Enumerable.Empty<Document>();
        }
    }
}