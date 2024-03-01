﻿namespace InventoryStudio.Services.File
{
    public interface IFileParser
    {
        Task<List<Dictionary<string, string>>> Parse(IFormFile file);
    }
}