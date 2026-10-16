namespace SharpExplorer.Controls;

public static class MeasuringHelper
{
    public static readonly StringFormat FormatForMeasuring;

    static MeasuringHelper()
    {
        FormatForMeasuring = new(StringFormat.GenericTypographic)
        {
            Alignment = StringAlignment.Center,
            Trimming = StringTrimming.None
        };
        FormatForMeasuring.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
    }
}