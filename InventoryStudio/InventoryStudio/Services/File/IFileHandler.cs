namespace InventoryStudio.File
{
    public interface IFileHandler<T> where T : class
    {
        Task<byte[]> Export(params IEnumerable<T>[] recordLists);
        Task<List<T>> Import(IFormFile file);
    }
}
