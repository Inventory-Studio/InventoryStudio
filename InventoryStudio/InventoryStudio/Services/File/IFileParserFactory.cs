namespace InventoryStudio.Services.File
{
    public interface IFileParserFactory
    {
        IFileParser CreateParser(string fileType);
    }
}
