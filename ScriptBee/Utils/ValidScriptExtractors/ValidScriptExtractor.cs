using System;

namespace ScriptBee.Utils.ValidScriptExtractors
{
    public abstract class ValidScriptExtractor
    {
        protected abstract string StartComment { get; }

        protected abstract string EndComment { get; }

        public string ExtractValidScript(string script)
        {
            if (string.IsNullOrEmpty(script))
            {
                return "";
            }

            int startPosition = script.IndexOf(StartComment, StringComparison.Ordinal);
            int endPosition = script.IndexOf(EndComment, StringComparison.Ordinal);

            if (startPosition >= 0 && endPosition > 0)
            {
                return script.Substring(startPosition + StartComment.Length,
                    endPosition - startPosition - StartComment.Length).Trim();
            }

            return "";
        }
    }
}