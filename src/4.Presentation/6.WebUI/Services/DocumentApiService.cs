using PocLineAPI.Presentation.WebUI.Models;
using System.Text.Json;
using System.Text;

namespace PocLineAPI.Presentation.WebUI.Services
{
    public interface IDocumentApiService
    {
        Task<List<DocumentViewModel>> GetAllAsync();
        Task<DocumentViewModel?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(DocumentViewModel document);
        Task<bool> UpdateAsync(DocumentViewModel document);
        Task<bool> DeleteAsync(Guid id);
    }

    public class DocumentApiService : IDocumentApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public DocumentApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:7123";
        }

        public async Task<List<DocumentViewModel>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/document");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var documents = JsonSerializer.Deserialize<List<DocumentViewModel>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return documents ?? new List<DocumentViewModel>();
                }
                return new List<DocumentViewModel>();
            }
            catch
            {
                return new List<DocumentViewModel>();
            }
        }

        public async Task<DocumentViewModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/document/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var document = JsonSerializer.Deserialize<DocumentViewModel>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return document;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateAsync(DocumentViewModel document)
        {
            try
            {
                var json = JsonSerializer.Serialize(document);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/document", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(DocumentViewModel document)
        {
            try
            {
                var json = JsonSerializer.Serialize(document);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"{_baseUrl}/api/document/{document.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/document/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}