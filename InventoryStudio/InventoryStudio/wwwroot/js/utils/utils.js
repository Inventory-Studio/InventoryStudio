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
            if (pdfdata?.length > 0) {
                var exportProperties = {
                    dataSource: pdfdata,
                    includeHiddenColumn: true,
                    fileName: fileName + ".pdf"
                };
                gridObj.pdfExport(exportProperties);
            } else {
                gridObj.pdfExport({
                    dataSource: gridObj.dataSource,
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
                    dataSource: gridObj.dataSource,
                    fileName: fileName + ".xlsx"
                });

            }
        }
    }
}

function customBarcodeExportToolbarClick(gridId, fileName) {
    return (args) => {
        var gridObj = document.getElementById(gridId).ej2_instances[0];
        const gridPdfExport = gridId + "_pdfexport";
        const gridExcelExport = gridId + "_excelexport";
        if (args.item.id === gridPdfExport) {
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
                    dataSource: gridObj.dataSource,
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
                    dataSource: gridObj.dataSource,
                    fileName: fileName + ".xlsx"
                });

            }
        }
    }
}

function getFormData($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};

    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });

    return indexed_array;
}