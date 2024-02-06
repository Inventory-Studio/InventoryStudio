namespace InventoryStudio.File
{
    public class ExcelFileHandler<T> : IFileHandler<T>
    {
        public Task<byte[]> DownloadFileAsync(IEnumerable<T> entity)
        {
            throw new NotImplementedException();
        }

        public Task UploadFileAsync(IFormFile file)
        {
            return Task.CompletedTask;
        }
    }

}
