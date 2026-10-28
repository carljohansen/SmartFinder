using RangeFinder.Domain.History;

namespace RangeFinder.Controls;

public class PredictiveComboBox : ComboBox
{
    public void FocusWithEndCursor()
    {
        Focus();
        MoveCursorToEnd();
    }

    public void MoveCursorToEnd()
    {
        SelectionStart = Text.Length;
        SelectionLength = 0;
    }

    public bool ApplyComboBoxHistory(TextCriterionHistory history)
    {
        int? selectedIndexToRestore = null;
        if (SelectedIndex >= this.Items.Count)
        {
            selectedIndexToRestore = SelectedIndex;
            SelectedIndex = -1;
        }

        if (Text != "")
        {
            return false;
        }
        Items.Clear();
        foreach (var item in history.Items)
        {
            Items.Add(item);
        }

        if (selectedIndexToRestore.HasValue && selectedIndexToRestore.Value < Items.Count)
        {
            SelectedIndex = selectedIndexToRestore.Value;
        }
        return true;
    }

    public void PopulateDropdown(TextCriterionHistory history, IEnumerable<string> continuations)
    {
        var caretPos = SelectionStart;
        var currText = Text;

        if (!ApplyComboBoxHistory(history))
        {
            Items.Clear();
            //Items.Add(currText);
            foreach (var continuation in continuations)
            {
                Items.Add(continuation);
            }
        }

        if (Items.Count > 0)
        {
            DroppedDown = true;
        }

        SelectionStart = Math.Min(caretPos, Text.Length);
    }
}
