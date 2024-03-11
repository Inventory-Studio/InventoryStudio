﻿using InventoryStudio.Services.FileHandlers;

namespace InventoryStudio.Services.Importers
{
    public interface IImporter
    {
        Task ImportDataAsync(string companyId, string importTemplateID, string userId, ProgressHandler progressHandler, List<Dictionary<string, string>> datas);



    }

    public interface IMultipleLineImporter
    {
        Task ImportDatasAsync(string companyId, string importTemplateID, string userId, ProgressHandler progressHandler, List<List<Dictionary<string, string>>> datas);
    }
}
