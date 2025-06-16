using Microsoft.Extensions.DependencyInjection;
using PocLineAPI.Application.Interfaces;
using PocLineAPI.Application.Services;
using PocLineAPI.Domain.Entities;
using PocLineAPI.Domain.Interfaces;
using PocLineAPI.Infrastructure.Repositories;
using PocLineAPI.Infrastructure.Services;
using Serilog;
using System.IO;

namespace PocLineAPI.Presentation.Evaluator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "src/4.Presentation/7.Evaluator/Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 31, // Keep logs for the last 31 days
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 10_000_000, // Optional: Limit file size (10MB per file)
                    shared: true
                )
                .CreateLogger();

            Log.Information("Starting Qdrant Document Evaluator");

            // Setup dependency injection
            var serviceProvider = new ServiceCollection()
                .AddScoped<IEmbeddingService, OpenAIEmbeddingService>()
                .AddScoped<IRepository, QdrantRepository>()
                .AddScoped<IDocumentService, DocumentService>()
                .BuildServiceProvider();

            var documentService = serviceProvider.GetRequiredService<IDocumentService>();

            Log.Information("Qdrant Document Evaluator initialized");

            Console.WriteLine("Qdrant Document Evaluator");
            Console.WriteLine("=======================");

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nPlease select an option:");
                Console.WriteLine("1. Add a document");
                Console.WriteLine("2. Search for documents");
                Console.WriteLine("3. List all documents");
                Console.WriteLine("4. Exit");
                Console.Write("\nYour choice: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await AddDocumentAsync(documentService);
                        break;
                    case "2":
                        await SearchDocumentsAsync(documentService);
                        break;
                    case "3":
                        await ListAllDocumentsAsync(documentService);
                        break;
                    case "4":
                        exit = true;
                        Log.Information("User requested to exit the application");
                        break;
                    default:
                        Log.Warning("Invalid option selected: {Choice}", choice);
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }
            }

            Console.WriteLine("Thank you for using the Qdrant Document Evaluator!");
            Log.Information("Shutting down Qdrant Document Evaluator");
            Log.CloseAndFlush();
        }

        private static async Task AddDocumentAsync(IDocumentService documentService)
        {
            Log.Debug("Entering AddDocumentAsync method");
            Console.WriteLine("\nAdd a new document");
            Console.Write("Document ID (or leave empty for auto-generated): ");
            var id = Console.ReadLine();
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
                Log.Debug("Auto-generated document ID: {Id}", id);
            }

            Console.Write("Document content: ");
            var content = Console.ReadLine() ?? string.Empty;

            var document = new Document(id, content);
            Log.Information("Attempting to add document with ID: {Id}", id);
            var result = await documentService.CreateDocumentAsync(document);

            if (result)
            {
                Log.Information("Document added successfully with ID: {Id}", id);
                Console.WriteLine($"Document added successfully with ID: {id}");
            }
            else
            {
                Log.Warning("Failed to add document with ID: {Id}", id);
                Console.WriteLine("Failed to add document.");
            }
        }

        private static async Task SearchDocumentsAsync(IDocumentService documentService)
        {
            Log.Debug("Entering SearchDocumentsAsync method");
            Console.WriteLine("\nSearch for documents");
            Console.Write("Enter search query: ");
            var query = Console.ReadLine() ?? string.Empty;
            Log.Information("Searching documents with query: {Query}", query);

            try
            {
                var results = await documentService.SearchSimilarDocumentsAsync(query);

                Log.Information("Search completed. Found {Count} results", results.Count());
                Console.WriteLine($"\nFound {results.Count()} results:");

                foreach (var doc in results)
                {
                    Console.WriteLine($"ID: {doc.Id}, Content: {doc.Content}");
                }
            }
            catch (NotImplementedException)
            {
                Log.Warning("Search functionality is not yet implemented");
                Console.WriteLine("Search functionality is not yet implemented.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred during document search");
                Console.WriteLine("An error occurred during search.");
            }
        }

        private static async Task ListAllDocumentsAsync(IDocumentService documentService)
        {
            Log.Debug("Entering ListAllDocumentsAsync method");
            Console.WriteLine("\nListing all documents");

            try
            {
                var documents = await documentService.GetAllDocumentsAsync();
                Log.Information("Retrieved {Count} documents", documents.Count());

                if (!documents.Any())
                {
                    Log.Information("No documents found in repository");
                    Console.WriteLine("No documents found.");
                    return;
                }

                foreach (var doc in documents)
                {
                    Console.WriteLine($"ID: {doc.Id}, Content: {doc.Content}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while listing documents");
                Console.WriteLine("An error occurred while retrieving documents.");
            }
        }
    }
}
