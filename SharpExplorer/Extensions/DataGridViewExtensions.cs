namespace SharpExplorer.Extensions;

internal static class DataGridViewExtensions
{
    extension(DataGridView dataGridView)
    {
        public IEnumerable<DataGridViewColumn> ColumnsEnumerable()
        {
            for (var i = 0; i < dataGridView.Columns.Count; i++)
            {
                yield return dataGridView.Columns[i];
            }
        }
    }
}
