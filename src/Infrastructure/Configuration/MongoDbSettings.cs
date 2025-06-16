namespace PocLineAPI.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration settings for MongoDB connection.
    /// </summary>
    public class MongoDbSettings
    {
        public const string SectionName = "MongoDbSettings";
        
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string DocumentCollection { get; set; } = "documents";
        public string LineEventCollection { get; set; } = "line_events";
        public string LineMessageCollection { get; set; } = "line_messages";
    }
}