namespace InventoryStudio.File
{
    public interface IFileHandler<T> where T : class
    {
        Task<byte[]> Export(params IEnumerable<T>[] recordLists);
        Task<List<T>> Import(IFormFile file);

        Task<string[]> GetHeader(IFormFile file);

        Task<Dictionary<string, string>> MapHeadersToEntityProperties(string[] headerFields);
    }
}
