function normalizedBoolean(field, data, column) {
    const value = data[field];
    if (value) {
        return "Yes";
    }
    return "No";
}

function customExportToolbarClick(gridId, fileName) {
    return (args) => {
        var gridObj = document.getElementById(gridId).ej2_instances[0];
        const gridPdfExport = gridId + "_pdfexport";
        const gridExcelExport = gridId + "_excelexport";
        if (args.item.id === gridPdfExport) {
            var pdfdata = gridObj.getSelectedRecords();
            console.log(pdfdata)
            if (pdfdata?.length > 0) {
                var exportProperties = {
                    dataSource: pdfdata,
                    includeHiddenColumn: true,
                    fileName: fileName + ".pdf"
                };
                gridObj.pdfExport(exportProperties);
            } else {
                gridObj.pdfExport({
                    fileName: fileName + ".pdf"
                });
            }
        }
        if (args.item.id === gridExcelExport) {
            var exceldata = gridObj.getSelectedRecords();
            if (exceldata?.length > 0) {
                var exportProperties = {
                    dataSource: exceldata,
                    includeHiddenColumn: true,
                    fileName: fileName + ".xlsx"
                };
                gridObj.excelExport(exportProperties);
            } else {
                gridObj.excelExport({
                    fileName: fileName + ".xlsx"
                });

            }
        }
    }
}
