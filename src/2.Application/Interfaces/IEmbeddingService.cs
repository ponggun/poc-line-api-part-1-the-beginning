namespace PocLineAPI.Application.Interfaces
{
    /// <summary>
    /// Service for creating embeddings from text.
    /// </summary>
    public interface IEmbeddingService
    {
        /// <summary>
        /// Creates a vector embedding from text.
        /// </summary>
        /// <param name="text">The text to embed</param>
        /// <returns>A vector representation of the input text</returns>
        Task<float[]> CreateEmbeddingAsync(string text);
        
        /// <summary>
        /// Creates vector embeddings for multiple texts.
        /// </summary>
        /// <param name="texts">The texts to embed</param>
        /// <returns>Vector representations of the input texts</returns>
        Task<IList<float[]>> CreateEmbeddingsAsync(IList<string> texts);
    }
}