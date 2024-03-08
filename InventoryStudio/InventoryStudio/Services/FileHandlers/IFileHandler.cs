namespace InventoryStudio.FileHandlers
{
    public interface IFileHandler
    {
        Task<List<string[]>> ImportTemplate(IFormFile file);

        //【Todo】Need to adjust support for creating multiple objects
        Task<byte[]> ExportTemplate(string[] headerFields);

        Task<byte[]> ExportImportResult(List<string> datas);

        Task<List<Dictionary<string, string>>> ImportData(IFormFile file);

        Task<List<Dictionary<string, string>>> ImportDatas(IFormFile file);
    }
}
