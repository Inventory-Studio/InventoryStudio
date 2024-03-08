using ISLibrary.ImportTemplateManagement;

namespace InventoryStudio.Services.Importers
{
    public abstract class BaseImporter
    {
        protected bool TryGetDestinationField(string sourceField, List<ImportTemplateField> importTemplateFields, out string destinationField)
        {
            var field = importTemplateFields.FirstOrDefault(f => f.SourceField == sourceField);
            destinationField = field?.DestinationField;
            return field != null;
        }
    }
}
