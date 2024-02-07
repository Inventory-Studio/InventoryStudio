namespace InventoryStudio.File
{
    public interface IFileHandler<T>
    {
        Task<byte[]> DownloadFileAsync(IEnumerable<T> records);
        Task<List<T>> UploadFileAsync(IFormFile file);
    }
}
