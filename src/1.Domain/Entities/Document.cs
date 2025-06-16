namespace PocLineAPI.Domain.Entities
{
    /// <summary>
    /// Represents a document that can be stored and retrieved from the vector database.
    /// </summary>
    public class Document
    {
        public string Id { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        public Document()
        {
        }

        public Document(string id, string content, Dictionary<string, string>? metadata = null)
        {
            Id = id;
            Content = content;
            Metadata = metadata ?? new Dictionary<string, string>();
        }
    }
}