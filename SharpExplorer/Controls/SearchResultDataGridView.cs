using SharpExplorer.Domain;
using System.Data;

namespace SharpExplorer.Controls;

public class SearchResultDataGridView : DataGridView
{
    public int LocationColumnIndex => _locationColIndex;

    private int _locationColIndex = -1;   

    public void Initialize()
    {
        var locationCol = Columns["colLocation"] ?? throw new ArgumentNullException("Grid columns not initialized.");
        _locationColIndex = locationCol.Index!;
    }

    public void ResizeResultsGrid(int newWidth)
    {
        if (_locationColIndex < 0)
        {
            throw new ArgumentNullException("Not initialized");
        }

        var fixedColWidths = 0;
        for (int i = 0; i < Columns.Count; i++)
        {
            if (i != _locationColIndex)
            {
                fixedColWidths += Columns[i].Width;
            }
        }

        if (fixedColWidths == 0)
        {
            return;
        }

        Columns[_locationColIndex].Width = Math.Max(50, newWidth - fixedColWidths);
    }

    public FileDisplay? GetRowFileDisplay(int rowIndex)
    {
        if (rowIndex < 0)
            return null;
        var selectedRow = Rows[rowIndex];
        return (selectedRow.DataBoundItem is DataRowView view && view.Row["FileDisplay"] is FileDisplay fileDisplay) ? fileDisplay : null;
    }

    private float _gridTextBenchmarkWidth = 0; // The width of a benchmark 10-character string.

    public int GetGridCellCapacity(int cellWidth)
    {
        // Return the number of characters that can fit in a cell of the given width, based on the benchmark width.
        if (_gridTextBenchmarkWidth == 0)
        {
            using Graphics g = CreateGraphics();
            _gridTextBenchmarkWidth = g.MeasureString(@"C:\Wapita\", Font, 0, MeasuringHelper.FormatForMeasuring).Width;
        }

        return (int)(cellWidth / _gridTextBenchmarkWidth * 10) - 4; // Allow for some padding and ellipsis.
    }

    public int GetCurrentGridCellCapacity(int colIndex)
    {
        return GetGridCellCapacity(Columns[colIndex].Width);
    }

    public int GetCurrentLocationColumnCapacity()
    {
        return GetCurrentGridCellCapacity(_locationColIndex);
    }

    internal void FocusAfterSearch()
    {
        var numMatches = Rows.Count;
        if (numMatches > 0)
        {
            Rows[0].Selected = true;
            CurrentCell = Rows[0].Cells[0];
            Focus();
        }
    }
}
