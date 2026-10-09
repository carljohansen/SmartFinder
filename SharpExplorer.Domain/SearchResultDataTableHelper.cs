using System.Data;

namespace SharpExplorer.Domain;

public static class SearchResultDataTableHelper
{
    public static void CreateColumns(DataTable searchResultTable)
    {
        searchResultTable.Columns.Add(nameof(FileDisplay.Name), typeof(string));
        searchResultTable.Columns.Add(nameof(FileDisplay.SizeDisplay), typeof(FileSizeDisplayComparable));
        searchResultTable.Columns.Add(nameof(FileDisplay.Location), typeof(string));
        searchResultTable.Columns.Add(nameof(FileDisplay.FileTypeDescription), typeof(string));
        searchResultTable.Columns.Add(nameof(FileDisplay.LastModified), typeof(DateTime));
        searchResultTable.Columns.Add("FileDisplay", typeof(FileDisplay)); // hidden backing object
    }
}