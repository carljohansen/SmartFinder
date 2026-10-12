using System.Data;
using System.Drawing;

namespace SharpExplorer.Domain;

public static class SearchResultDataTableHelper
{
    public static DataTable CreateFileResultDataTableColumns()
    {
        DataTable searchResultTable = new();
        searchResultTable.Columns.Add(nameof(FileDisplay.Icon), typeof(Icon));
        searchResultTable.Columns.Add(nameof(FileDisplay.Name), typeof(string));
        searchResultTable.Columns.Add(nameof(FileDisplay.SizeDisplay), typeof(FileSizeDisplayComparable));
        searchResultTable.Columns.Add(nameof(FileDisplay.Location), typeof(string));
        searchResultTable.Columns.Add(nameof(FileDisplay.FileTypeDescription), typeof(string));
        searchResultTable.Columns.Add(nameof(FileDisplay.LastModified), typeof(DateTime));
        searchResultTable.Columns.Add("FileDisplay", typeof(FileDisplay)); // hidden backing object

        return searchResultTable;
    }
}