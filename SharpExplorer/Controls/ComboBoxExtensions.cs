using SharpExplorer.Domain;

namespace SharpExplorer.Controls;

internal static class ComboBoxExtensions
{
    extension(ComboBox comboBox)
    {
        public void SetHistory(TextCriterionHistory history)
        {
            comboBox.Items.Clear();
            foreach (var item in history.Items)
            {
                comboBox.Items.Add(item);
            }
        }
    }
}
