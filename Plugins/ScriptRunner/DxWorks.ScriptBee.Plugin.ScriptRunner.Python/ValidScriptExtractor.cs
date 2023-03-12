using static DxWorks.ScriptBee.Plugin.ScriptRunner.Python.ValidScriptDelimiters;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Python;

internal static class ValidScriptExtractor
{
    public static string ExtractValidScript(string script)
    {
        if (string.IsNullOrEmpty(script))
        {
            return "";
        }

        var startPosition = script.IndexOf(PythonStartComment, StringComparison.Ordinal);
        var endPosition = script.IndexOf(PythonEndComment, StringComparison.Ordinal);

        if (startPosition >= 0 && endPosition > 0)
        {
            return script.Substring(startPosition + PythonStartComment.Length,
                endPosition - startPosition - PythonStartComment.Length).Trim();
        }

        return "";
    }
}
