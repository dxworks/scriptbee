using static DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript.ValidScriptDelimiters;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript;

internal static class ValidScriptExtractor
{
    public static string ExtractValidScript(string script)
    {
        if (string.IsNullOrEmpty(script))
        {
            return "";
        }

        var startPosition = script.IndexOf(JavascriptStartComment, StringComparison.Ordinal);
        var endPosition = script.IndexOf(JavascriptEndComment, StringComparison.Ordinal);

        if (startPosition >= 0 && endPosition > 0)
        {
            return script.Substring(startPosition + JavascriptStartComment.Length,
                endPosition - startPosition - JavascriptStartComment.Length).Trim();
        }

        return "";
    }
}
