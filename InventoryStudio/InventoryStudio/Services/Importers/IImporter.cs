using InventoryStudio.Services.FileHandlers;

namespace InventoryStudio.Services.Importers
{
    public interface IImporter
    {
        Task ImportDataAsync(string companyId, string importTemplateID, string userId, ProgressHandler progressHandler, List<Dictionary<string, string>> datas);
    }
}
