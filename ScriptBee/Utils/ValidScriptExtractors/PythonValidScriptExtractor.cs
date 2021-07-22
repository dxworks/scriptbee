using System;

namespace ScriptBee.Utils.ValidScriptExtractors
{
    public class PythonValidScriptExtractor : IValidScriptExtractor
    {
        public string ExtractValidScript(string script)
        {
            if (string.IsNullOrEmpty(script))
            {
                return "";
            }
            const string startComment = "# start script";
            int startPosition = script.IndexOf(startComment, StringComparison.Ordinal);
            const string endComment = "# end script";
            int endPosition = script.IndexOf(endComment, StringComparison.Ordinal);

            if (startPosition >= 0 && endPosition > 0)
            {
                return script.Substring(startPosition + startComment.Length,
                    endPosition - startPosition - startComment.Length).Trim();
            }

            return "";
        }
    }
}