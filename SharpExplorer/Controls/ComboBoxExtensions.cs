using SharpExplorer.Domain.History;

namespace SharpExplorer.Controls;

internal static class ComboBoxExtensions
{
    extension(ComboBox comboBox)
    {
        public void ApplyComboBoxHistory(TextCriterionHistory history)
        {
            comboBox.Items.Clear();
            foreach (var item in history.Items)
            {
                comboBox.Items.Add(item);
            }
        }
    }
}
