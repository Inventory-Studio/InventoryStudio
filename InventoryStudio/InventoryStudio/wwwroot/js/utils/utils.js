function normalizedBooleanFn(field, data, column) {
    const value = data[field];
    if (value) {
        return "Yes";
    }
    return "No";
}

function customExportToolbarClick(gridId, fileName) {
    return (args) => {
        var gridObj = document.getElementById(gridId).ej2_instances[0];
        if (args.item.id === 'Grid_pdfexport') {
            var pdfdata = gridObj.getSelectedRecords();
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
        if (args.item.id === 'Grid_excelexport') {
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
