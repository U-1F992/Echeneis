using System.Windows.Automation;

public static class AutomationElementExtension
{
    public static void Invoke(this AutomationElement ae)
    {
        try
        {
            var ip = (InvokePattern)ae.GetCurrentPattern(InvokePattern.Pattern);
            ip?.Invoke();
        }
        catch { }
    }

    public static void SetValue(this AutomationElement ae, string value)
    {
        try
        {
            var vp = (ValuePattern)ae.GetCurrentPattern(ValuePattern.Pattern);
            vp?.SetValue(value);
        }
        catch { }
    }

    public static void Select(this AutomationElement ae)
    {
        try
        {
            var sip = (SelectionItemPattern)ae.GetCurrentPattern(SelectionItemPattern.Pattern);
            sip?.Select();
        }
        catch { }
    }

    public static string GetText(this AutomationElement ae)
    {
        object patternObj;
        if (ae.TryGetCurrentPattern(ValuePattern.Pattern, out patternObj))
        {
            var valuePattern = (ValuePattern)patternObj;
            return valuePattern.Current.Value;
        }
        else if (ae.TryGetCurrentPattern(TextPattern.Pattern, out patternObj))
        {
            var textPattern = (TextPattern)patternObj;
            return textPattern.DocumentRange.GetText(-1).TrimEnd('\r'); // often there is an extra '\r' hanging off the end.
        }
        else
        {
            return ae.Current.Name;
        }
    }
}
