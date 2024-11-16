namespace Dalamud.DrunkenToad.Extensions;

using Lumina.Text.ReadOnly;

public static class ReadOnlySeStringExtensions
{
    public static string ToRawString(this ReadOnlySeString str)
    {
        try
        {
            return str.ExtractText();
        }
        catch
        {
            return string.Empty;
        }
    }
}
