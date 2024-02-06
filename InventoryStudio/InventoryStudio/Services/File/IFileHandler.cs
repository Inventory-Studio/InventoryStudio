namespace InventoryStudio.File
{
    public interface IFileHandler<T>
    {
        Task UploadFileAsync(IFormFile file);
        Task<byte[]> DownloadFileAsync(IEnumerable<T> entity);
    }
}
